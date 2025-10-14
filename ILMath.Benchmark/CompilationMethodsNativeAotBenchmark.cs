using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace ILMath.Benchmark;

[SimpleJob(RuntimeMoniker.NativeAot80)]
public class CompilationMethodsNativeAotBenchmark {
    private IEvaluationContext<double> context = null!;
    private Evaluator<double> expressionTreeEvaluator = null!;
    private Evaluator<double> functionalEvaluator = null!;

    [GlobalSetup]
    public void Setup() {
        this.context = EvaluationContexts.CreateForDouble();
        this.expressionTreeEvaluator = MathEvaluation.CompileExpression<double>(string.Empty, Constant.Expression, CompilationMethod.ExpressionTree);
        this.functionalEvaluator = MathEvaluation.CompileExpression<double>(string.Empty, Constant.Expression, CompilationMethod.Functional);
    }

    [Benchmark]
    public Evaluator<double> ExpressionTreeCompilation() {
        return MathEvaluation.CompileExpression<double>(string.Empty, Constant.Expression, CompilationMethod.ExpressionTree);
    }

    [Benchmark]
    public Evaluator<double> FunctionalCompilation() {
        return MathEvaluation.CompileExpression<double>(string.Empty, Constant.Expression, CompilationMethod.Functional);
    }

    [Benchmark]
    public double ExpressionTreeEvaluation() {
        return this.expressionTreeEvaluator(this.context);
    }

    [Benchmark]
    public double FunctionalEvaluation() {
        return this.functionalEvaluator(this.context);
    }
}