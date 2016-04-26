#I "../packages/"
#r "Rx-Interfaces/lib/net45/System.Reactive.Interfaces.dll"
#r "Rx-Core/lib/net45/System.Reactive.Core.dll"
#r "Rx-Linq/lib/net45/System.Reactive.Linq.dll"
#r "FSharp.Control.Reactive/lib/net45/FSharp.Control.Reactive.dll"
#r "FSharp.Control.AsyncSeq/lib/net45/FSharp.Control.AsyncSeq.dll"

open System
open System.Timers
open FSharp.Control.Reactive

type Source = TA | ATS | BadTicks
type Currency = PLN | EUR | USD
type Pair = Currency * Currency
type Tick = { Ask: decimal; Bid: decimal; Pair: Pair; TimeStamp: DateTime; Source: Source }

let emptyEurPlnTick() =
    { Ask = 0m; Bid = 0m; Pair = (EUR, PLN); Source = TA; TimeStamp = DateTime.Now }

let random = Random()
let randomValue() = 1m + 5m * decimal(random.NextDouble())

let randomTick() = { emptyEurPlnTick() with Ask = randomValue(); Bid = randomValue(); }

let randomTaTick() = { randomTick() with Source = TA }
let randomAtsTick() = { randomTick() with Source = ATS }

let createTimerAndObservable timerInterval =
    let timer = new Timer(float timerInterval)
    timer.AutoReset <- true

    let task = async {
        timer.Start()
        do! Async.Sleep 2000
        timer.Stop()
    }

    (task, timer.Elapsed)

let timer1, eventStream1 = createTimerAndObservable 10
let timer2, eventStream2 = createTimerAndObservable 100

let taStream = eventStream1 |> Observable.map (fun _ -> randomTaTick())
let atsStream = eventStream2 |> Observable.map (fun _ -> randomAtsTick())

let checkLimit v = v > 4m
let defaultTick = { emptyEurPlnTick() with Bid = 4m; Source = BadTicks }

let preferTaIfMoreThenLimit (ta, ats) = if checkLimit ta.Bid then ta else ats
let checkLimitOrDefault tick = if checkLimit tick.Bid then tick else defaultTick

// =CHECK_TICK(PREFER_IF_VALID(PREFER_IF_VALID(_stream_, "TA"), "ATS"))
let formula = preferTaIfMoreThenLimit >> checkLimitOrDefault

Observable.combineLatest taStream atsStream
|> Observable.throttle (TimeSpan.FromMilliseconds(10.0))
|> Observable.map formula
|> Observable.subscribe (fun tick -> printfn "bad tick stream: %A" tick)

let getMinMax ((ta: Tick), (ats: Tick)) =
    (Math.Min(ta.Ask, ats.Ask), Math.Max(ta.Bid, ats.Bid))

let minMaxStream =
    Observable.combineLatest taStream atsStream
    |> Observable.map getMinMax

minMaxStream |> Observable.subscribe (fun tick -> printfn "min max stream: %A" tick)

[timer1;timer2]
|> Async.Parallel
|> Async.RunSynchronously
