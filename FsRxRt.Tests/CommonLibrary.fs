namespace ReactiveQuotationEngine

module CommonLibrary =
    open ReactiveQuotationEngine.DomainTypes

    let isFromSource source (tick: InputTick) = tick.Source = source

    let isNotFromSource source (tick: InputTick) = not (isFromSource source tick)

    let ValidTick (tick: InputTick) = Valid { Ask = tick.Ask; Bid = tick.Bid; Pair = tick.Pair }