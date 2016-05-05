namespace ReactiveQuotationEngine

module MinMaxFormula =
    open System
    open ReactiveQuotationEngine
    open DomainTypes

    // BEGIN part editable via "GUI" (or generated from other representation)
    let getMinMaxFold (ask, bid) (tick: InputTick) =
        (Math.Min(tick.Ask, ask), Math.Max(tick.Bid, bid))

    let getMinMax (ticks: InputTick seq) =
        let (minAsk, maxBid) =
            ticks |> Seq.fold getMinMaxFold (Decimal.MaxValue, Decimal.MinValue)
        Valid { Ask = minAsk; Bid = maxBid; Pair = (ticks |> Seq.head).Pair }

    let formula = { Name = "Min Ask, Max Bid formula"; Definition = getMinMax }
    // END
