#I "../packages/"
#r "Rx-Interfaces/lib/net45/System.Reactive.Interfaces.dll"
#r "Rx-Core/lib/net45/System.Reactive.Core.dll"
#r "Rx-Linq/lib/net45/System.Reactive.Linq.dll"
#r "FSharp.Control.Reactive/lib/net45/FSharp.Control.Reactive.dll"
#r "FSharp.Control.AsyncSeq/lib/net45/FSharp.Control.AsyncSeq.dll"

#load "DomainTypes.fs"
#load "CommonLibrary.fs"
#load "BadTickFormula.fs"
#load "MinMaxFormula.fs"
#load "RandomSource.fs"

open System
open FSharp.Control.Reactive
open ReactiveQuotationEngine
open DomainTypes

let filterByPair pair (tick: InputTick) = tick.Pair = pair
let filterBySource source (tick: InputTick) = tick.Source = source

let timer, randomTicksStream = RandomSource.createRandomTickStream 10 2000
let randomEurPlnStream = randomTicksStream |> Observable.filter (filterByPair (EUR, PLN))
let taEurPlnStream = randomEurPlnStream |> Observable.filter (filterBySource TA)
let atsEurPlnStream = randomEurPlnStream |> Observable.filter (filterBySource ATS)
let availableStreams = [|taEurPlnStream; atsEurPlnStream|]

let applyFormula (formula: Formula) =
    availableStreams
    |> Observable.combineLatestArray
    |> Observable.throttle (TimeSpan.FromMilliseconds(10.0))
    |> Observable.map Seq.cast
    |> Observable.map formula.Definition
    |> Observable.subscribe (fun tick -> printfn "%A applied: %A" formula.Name tick)

//applyFormula BadTickFormula.formula
applyFormula MinMaxFormula.formula

Async.RunSynchronously timer
