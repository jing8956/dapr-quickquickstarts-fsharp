namespace Subtract.Controllers

open System
open Microsoft.AspNetCore.Mvc
open Subtract.Models

[<ApiController>]
[<Route("[controller]")>]
type SubtractController() =
    inherit ControllerBase()

    //POST: /subtract
    [<HttpPost>]
    member _.Subtract(operands: Operands) =
        Console.WriteLine($"Subtracting {operands.OperandTwo} from {operands.OperandOne}")
        Decimal.Parse(operands.OperandOne) - Decimal.Parse(operands.OperandTwo)
