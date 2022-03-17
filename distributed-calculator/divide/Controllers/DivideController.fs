namespace Divide.Controllers

open System
open Microsoft.AspNetCore.Mvc
open Divide.Models

[<ApiController>]
[<Route("[controller]")>]
type DivideController() =
    inherit ControllerBase()

    //POST: /divide
    [<HttpPost>]
    member _.Divide(operands: Operands) =
        Console.WriteLine($"Dividing {operands.OperandOne} by {operands.OperandTwo}")
        Decimal.Parse(operands.OperandOne) / Decimal.Parse(operands.OperandTwo)
