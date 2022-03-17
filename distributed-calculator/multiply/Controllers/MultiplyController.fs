namespace Multiply.Controllers

open System
open Microsoft.AspNetCore.Mvc
open Multiply.Models

[<ApiController>]
[<Route("[controller]")>]
type MultiplyController() =
    inherit ControllerBase()

    //POST: /multiply
    [<HttpPost>]
    member _.Multiply(operands: Operands) =
        Console.WriteLine($"Calculating {operands.OperandOne} * {operands.OperandTwo}")
        Decimal.Parse(operands.OperandOne) * Decimal.Parse(operands.OperandTwo)
