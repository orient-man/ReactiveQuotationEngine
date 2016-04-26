#I "../packages/"
#r "Rx-Interfaces/lib/net45/System.Reactive.Interfaces.dll"
#r "Rx-Core/lib/net45/System.Reactive.Core.dll"
#r "Rx-Linq/lib/net45/System.Reactive.Linq.dll"
#r "FSharp.Control.Reactive/lib/net45/FSharp.Control.Reactive.dll"
#r "FSharp.Control.AsyncSeq/lib/net45/FSharp.Control.AsyncSeq.dll"

#load "Domain.fs"
#load "RandomSource.fs"

open System
open FSharp.Control.Reactive
open Domain

let timer, randomTicksStream = RandomSource.createRandomTickStream 10 2000

let filterEurPln (tick: InputTick) = match tick with { Pair = (EUR, PLN)} -> Some(tick) | _ -> None
let filterTa (tick: InputTick) = match tick with { Source = TA } -> Some(tick) | _ -> None
let filterAts (tick: InputTick) = match tick with { Source = ATS } -> Some(tick) | _ -> None

let randomEurPlnStream = randomTicksStream |> Observable.choose filterEurPln
let taEurPlnStream = randomEurPlnStream |>  Observable.choose filterTa
let atsEurPlnStream = randomEurPlnStream |>  Observable.choose filterAts

let checkLimit v = v > 4m

let preferTaIfMoreThenLimit (ta: InputTick, ats: InputTick) =
    if checkLimit ta.Bid then ta else ats

let badTickIfLimitNotReached (tick: InputTick) =
    if checkLimit tick.Bid then Valid { Ask = tick.Ask; Bid = tick.Bid; Pair = tick.Pair }
    else BadTick "limit for bid not reached"

// =CHECK_TICK(PREFER_IF_VALID(PREFER_IF_VALID(_stream_, "TA"), "ATS"))

let formula = preferTaIfMoreThenLimit >> badTickIfLimitNotReached

Observable.combineLatest taEurPlnStream atsEurPlnStream
|> Observable.throttle (TimeSpan.FromMilliseconds(10.0))
|> Observable.map formula
|> Observable.subscribe (fun tick -> printfn "Bid checked stream: %A" tick)

let getMinMax ((ta: InputTick), (ats: InputTick)) =
    (Math.Min(ta.Ask, ats.Ask), Math.Max(ta.Bid, ats.Bid))

let minMaxStream =
    Observable.combineLatest taEurPlnStream atsEurPlnStream
    |> Observable.map getMinMax

//minMaxStream |> Observable.subscribe (fun tick -> printfn "min max stream: %A" tick)

Async.RunSynchronously timer
