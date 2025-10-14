using System.Diagnostics;
using System.Numerics;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using ILMath.Exception;
using ILMath.SyntaxTree;

namespace ILMath.Compiler;

/// <summary>
/// Compiles the expression by generating IL code.<br/>
/// <b>Note:</b> Does not work in AOT environments.
/// </summary>
public class IlCompiler<T> : ICompiler<T> where T : unmanaged, INumber<T> {
    private record struct CompilationState(LocalBuilder? Parameters, int StackLocation);

    /// <summary>
    /// Compiles the syntax tree into a function.
    /// </summary>
    /// <param name="name">The name of the function.</param>
    /// <param name="tree">The syntax tree to compile.</param>
    /// ///
    /// <returns>The evaluator.</returns>
    public Evaluator<T> Compile(string name, INode tree) {
        return this.CompileSyntaxTree(name, tree);
    }

    private Evaluator<T> CompileSyntaxTree(string name, INode rootNode) {
        // Calculate the maximum parameter stack size
        int maximumParameterStackSize = 0;
        CalculateMaximumParameterStackSize(rootNode, 0, ref maximumParameterStackSize);

        // Create a new dynamic method
        DynamicMethod method = new DynamicMethod(name, typeof(T), [typeof(IEvaluationContext<T>)]);
        ILGenerator il = method.GetILGenerator();

        // If maximum parameter stack size is greater than zero, stackalloc the parameters array
        LocalBuilder? parameters = null;
        if (maximumParameterStackSize > 0) {
            // Load the parameters count onto the stack
            il.Emit(OpCodes.Ldc_I4, maximumParameterStackSize * Unsafe.SizeOf<T>());

            // Create the parameters array on the stack
            il.Emit(OpCodes.Conv_U);
            il.Emit(OpCodes.Localloc);

            // Store the parameters array in a local variable
            parameters = il.DeclareLocal(typeof(T*));
            il.Emit(OpCodes.Stloc, parameters);
        }

        // Compile the syntax tree into IL code
        this.CompileNode(rootNode, il, new CompilationState(parameters, 0));

        // Return the value on the stack
        il.Emit(OpCodes.Ret);

        // Return a delegate from the dynamic method
        return (Evaluator<T>) method.CreateDelegate(typeof(Evaluator<T>));
    }

    private void CompileNode(INode node, ILGenerator il, CompilationState state) {
        switch (node) {
            case OperatorNode expressionNode: this.CompileOperatorNode(expressionNode, il, state); break;
            case NumberNode<T> numberNode:    CompileNumberNode(numberNode, il); break;
            case UnaryNode unaryNode:         this.CompileUnaryNode(unaryNode, il, state); break;
            case VariableNode variableNode:   this.CompileVariableNode(variableNode, il); break;
            case FunctionNode functionNode:   this.CompileFunctionNode(functionNode, il, state); break;
            default:                          throw new CompilerException($"Unknown node type: {node.GetType()}");
        }
    }

    private void CompileOperatorNode(OperatorNode operatorNode, ILGenerator il, CompilationState state) {
        INode left = operatorNode.Left;
        INode right = operatorNode.Right;
        OperatorType @operator = operatorNode.Operator;
        this.CompileNode(left, il, state);
        this.CompileNode(right, il, state);
        GenerateOperatorInstruction(@operator, il);
    }

    private static void CompileNumberNode(NumberNode<T> numberNode, ILGenerator il) {
        T value = numberNode.Value;
        if (typeof(T) == typeof(int))
            il.Emit(OpCodes.Ldc_I4, Unsafe.As<T, int>(ref value));
        else if (typeof(T) == typeof(uint))
            il.Emit(OpCodes.Ldc_I4, (int) Unsafe.As<T, uint>(ref value));
        else if (typeof(T) == typeof(long))
            il.Emit(OpCodes.Ldc_I8, Unsafe.As<T, long>(ref value));
        else if (typeof(T) == typeof(ulong))
            il.Emit(OpCodes.Ldc_I8, (long) Unsafe.As<T, ulong>(ref value));
        else if (typeof(T) == typeof(float))
            il.Emit(OpCodes.Ldc_R4, Unsafe.As<T, float>(ref value));
        else if (typeof(T) == typeof(double))
            il.Emit(OpCodes.Ldc_R8, Unsafe.As<T, double>(ref value));
    }

    private void CompileUnaryNode(UnaryNode unaryNode, ILGenerator il, CompilationState state) {
        this.CompileNode(unaryNode.Child, il, state);

        switch (unaryNode.Operator) {
            case OperatorType.Minus:          il.Emit(OpCodes.Neg); break;
            case OperatorType.OnesComplement: il.Emit(OpCodes.Not); break;
        }
    }

    private void CompileVariableNode(VariableNode variableNode, ILGenerator il) {
        // Load the context onto the stack
        il.Emit(OpCodes.Ldarg_0);

        // Load the variable identifier onto the stack
        il.Emit(OpCodes.Ldstr, variableNode.Identifier);

        // Call the GetVariable method
        il.Emit(OpCodes.Callvirt, typeof(IEvaluationContext<T>).GetMethod(nameof(IEvaluationContext<T>.GetVariable))!);
    }

    private void CompileFunctionNode(FunctionNode functionNode, ILGenerator il, CompilationState state) {
        // Load the context onto the stack
        il.Emit(OpCodes.Ldarg_0);

        // Load the function identifier onto the stack
        il.Emit(OpCodes.Ldstr, functionNode.Identifier);

        int parametersCount = functionNode.Parameters.Count;
        if (parametersCount > 0) {
            // Make sure the parameters array is not null
            Debug.Assert(state.Parameters != null);

            // Populate the parameters array
            for (int i = 0; i < parametersCount; i++) {
                // Load the parameters array pointer onto the stack
                il.Emit(OpCodes.Ldloc, state.Parameters);

                int offset = state.StackLocation + i;
                if (offset > 0) {
                    // Load the current byte offset onto the stack
                    il.Emit(OpCodes.Ldc_I4, offset * Unsafe.SizeOf<T>());

                    // Add the byte offset to the array pointer
                    il.Emit(OpCodes.Add);
                }

                // Compile the parameter node
                this.CompileNode(functionNode.Parameters[i], il, state with { StackLocation = offset });

                // Store the parameter value in the array
                if (typeof(T) == typeof(double))
                    il.Emit(OpCodes.Stind_R8);
                else if (typeof(T) == typeof(float))
                    il.Emit(OpCodes.Stind_R4);
                else if (typeof(T) == typeof(long) || typeof(T) == typeof(ulong))
                    il.Emit(OpCodes.Stind_I8);
                else
                    il.Emit(OpCodes.Stind_I4);
            }

            // Load the parameters array pointer onto the stack
            il.Emit(OpCodes.Ldloc, state.Parameters);

            int parametersOffset = state.StackLocation;
            if (parametersOffset > 0) {
                // Load the current byte offset onto the stack
                il.Emit(OpCodes.Ldc_I4, parametersOffset * Unsafe.SizeOf<T>());

                // Add the byte offset to the array pointer
                il.Emit(OpCodes.Add);
            }

            // Load the parameters count onto the stack
            il.Emit(OpCodes.Ldc_I4, parametersCount);

            // Create a span from the parameters array
            il.Emit(OpCodes.Newobj, typeof(Span<T>).GetConstructor(new[] { typeof(void*), typeof(int) })!);
        }
        else {
            // Create an empty span
            // We use Call instead of Ldsfld because the Span<T>.Empty is a property
            il.Emit(OpCodes.Call, typeof(Span<T>).GetProperty(nameof(Span<T>.Empty))!.GetMethod!);
        }

        // Call the CallFunction method
        il.Emit(OpCodes.Callvirt, typeof(IEvaluationContext<T>).GetMethod(nameof(IEvaluationContext<T>.CallFunction))!);
    }

    private static void GenerateOperatorInstruction(OperatorType operatorType, ILGenerator il) {
        il.Emit(GetOpCode(operatorType));
    }

    private static OpCode GetOpCode(OperatorType operatorType) {
        switch (operatorType) {
            case OperatorType.Plus:           return OpCodes.Add;
            case OperatorType.Minus:          return OpCodes.Sub;
            case OperatorType.Multiplication: return OpCodes.Mul;
            case OperatorType.Division:       return OpCodes.Div;
            case OperatorType.Modulo:         return OpCodes.Rem;
        }

        if (!Util<T>.IsFP) {
            switch (operatorType) {
                case OperatorType.Xor:            return OpCodes.Xor;
                case OperatorType.LShift:         return OpCodes.Shl;
                case OperatorType.RShift:         return Util<T>.IsUN ? OpCodes.Shr_Un : OpCodes.Shr;
                case OperatorType.And:            return OpCodes.And;
                case OperatorType.Or:             return OpCodes.Or;
                case OperatorType.OnesComplement: return OpCodes.Not;
            }
        }

        throw new CompilerException($"Unknown operator type '{operatorType}'");
    }

    private static void CalculateMaximumParameterStackSize(INode node, int stackLocation, ref int maximumStackSize) {
        if (node is FunctionNode functionNode) {
            // For each child, increment the stack size
            foreach (INode child in functionNode.Parameters) {
                stackLocation++;
                CalculateMaximumParameterStackSize(child, stackLocation, ref maximumStackSize);
                maximumStackSize = Math.Max(maximumStackSize, stackLocation + 1);
            }
        }
        else
            foreach (INode child in node.EnumerateChildren())
                CalculateMaximumParameterStackSize(child, stackLocation, ref maximumStackSize);
    }
}