namespace Add.Controllers

open System
open Microsoft.AspNetCore.Mvc
open Add.Models

[<ApiController>]
[<Route("[controller]")>]
type AddController() =
    inherit ControllerBase()

    //POST: /add
    [<HttpPost>]
    member _.Add(operands: Operands) =
        Console.WriteLine($"Adding {operands.OperandOne} to {operands.OperandTwo}")
        Decimal.Parse(operands.OperandOne) + Decimal.Parse(operands.OperandTwo)
