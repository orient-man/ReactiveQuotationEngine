namespace ReactiveQuotationEngine

module BadTickFormula =
    open ReactiveQuotationEngine
    open DomainTypes
    open CommonLibrary

    // BEGIN part editable via "GUI" (or generated from other representation)
    let checkLimit v = v > 3m

    let checkBidLimit (tick: InputTick) = checkLimit tick.Bid

    let isTA = isFromSource TA

    let isNotTA = isNotFromSource TA

    let preferTaIfMoreThenLimit (ticks: InputTick seq) =
        match ticks |> Seq.filter isTA |> Seq.tryFind checkBidLimit with
        | Some(ta) -> ta
        | None -> ticks |> Seq.find isNotTA

    let badTickIfLimitNotReached (tick: InputTick) =
        if checkLimit tick.Bid then ValidTick tick else BadTick "limit for bid not reached"

    let formula = {
        Name = "Bad tick formula"
        Definition = preferTaIfMoreThenLimit >> badTickIfLimitNotReached
    }
    // END