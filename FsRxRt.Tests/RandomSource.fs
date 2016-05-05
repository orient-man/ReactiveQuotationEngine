namespace ReactiveQuotationEngine

module RandomSource =

    open System
    open System.Timers
    open ReactiveQuotationEngine.DomainTypes

    let random = Random()

    let randomValue() = 1m + 5m * decimal(random.NextDouble())

    let randomSource() =
        let sources = [|TA; ATS|]
        sources.[random.Next(0, sources.Length)]

    let rec randomPair() =
        let currencies = [|PLN; EUR; USD|]
        let randomCurrency() = currencies.[random.Next(0, currencies.Length)]
        let pair = (randomCurrency(), randomCurrency())

        match pair with (x, y) when x = y -> randomPair() | _ -> pair

    let randomTick() : InputTick = {
        Ask = randomValue()
        Bid = randomValue()
        Pair = randomPair()
        Source = randomSource()
        TimeStamp = DateTime.Now
    }

    let createTimerAndObservable timerInterval elapsedTime =
        let timer = new Timer(float timerInterval)
        timer.AutoReset <- true

        let task = async {
            timer.Start()
            do! Async.Sleep elapsedTime
            timer.Stop()
        }

        (task, timer.Elapsed)

    let createRandomTickStream timerInterval elapsedTime =
        let timer, eventStream = createTimerAndObservable timerInterval elapsedTime
        let randomTicksStream = eventStream |> Observable.map (fun _ -> randomTick())
        (timer, randomTicksStream)