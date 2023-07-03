using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

using SemanticKernelDemo.Web.Controllers.Models;

namespace SemanticKernelDemo.Web.Controllers;

[ApiController]
[Produces(@"application/json")]
[Route(@"api/[controller]")]
public class DemoController : ControllerBase
{
    private readonly IKernel kernel;

    public DemoController(IKernel kernel)
    {
        this.kernel = kernel;
    }

    [HttpGet(@"hello")]
    public async Task<IActionResult> SayHelloAsync(CancellationToken cancellationToken)
    {
        var context = await kernel.RunAsync(cancellationToken, kernel.Skills.GetFunction(@"HelloPlugin", @"SayHello"));

        return context.ErrorOccurred
                ? Problem(context.LastErrorDescription)
                : Ok(context.Result.Trim());
    }

    [HttpPost(@"summarize")]
    public async Task<IActionResult> SummarizeAsync([FromBody] SummarizeRequest request, CancellationToken cancellationToken)
    {
        var variables = new ContextVariables();
        variables.Set(@"input", request.Input);

        var context = await kernel.RunAsync(variables, cancellationToken, kernel.Skills.GetFunction(@"SummarizePlugin", @"Summarize"));

        return context.ErrorOccurred
                ? Problem(context.LastErrorDescription)
                : Ok(context.Result.Trim());
    }

    [HttpPost(@"ask")]
    public async Task<IActionResult> AskAsync([FromBody] AskRequest request, CancellationToken cancellationToken)
    {
        var variables = new ContextVariables();
        variables.Set(@"input", request.Question);
        variables.Set(@"context", request.Context);

        var context = await kernel.RunAsync(variables, cancellationToken, kernel.Skills.GetFunction(@"AskPlugin", @"AnswerQuestions"));

        return context.ErrorOccurred
                ? Problem(context.LastErrorDescription)
                : Ok(context.Result.Trim());
    }

    [HttpPost(@"summarize/ask")]
    public async Task<IActionResult> AskWithSummarizedContextAsync([FromBody] AskRequest request, CancellationToken cancellationToken)
    {
        var variables = new ContextVariables();
        variables.Set(@"context", request.Context);
        variables.Set(@"input", request.Question);

        var context = await kernel.RunAsync(variables, cancellationToken, kernel.Skills.GetFunction(@"AskPlugin", @"AnswerQuestionsSummarizeContext"));

        return context.ErrorOccurred
                ? Problem(context.LastErrorDescription)
                : Ok(context.Result.Trim());
    }
}
