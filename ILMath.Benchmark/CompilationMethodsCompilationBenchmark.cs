using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace ILMath.Benchmark;

[SimpleJob(RuntimeMoniker.Net80)]
public class CompilationMethodsCompilationBenchmark {
    private EvaluationContext<double> context = null!;

    [GlobalSetup]
    public void Setup() {
        this.context = EvaluationContexts.CreateForDouble();
    }

    [Benchmark]
    public Evaluator<double> IntermediateLanguageCompilation() {
        return MathEvaluation.CompileExpression<double>(string.Empty, Constant.Expression, CompilationMethod.IntermediateLanguage);
    }

    [Benchmark]
    public Evaluator<double> ExpressionTreeCompilation() {
        return MathEvaluation.CompileExpression<double>(string.Empty, Constant.Expression, CompilationMethod.ExpressionTree);
    }

    [Benchmark]
    public Evaluator<double> FunctionalCompilation() {
        return MathEvaluation.CompileExpression<double>(string.Empty, Constant.Expression, CompilationMethod.Functional);
    }
}