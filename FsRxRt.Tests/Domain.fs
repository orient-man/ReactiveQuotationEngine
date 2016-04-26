module Domain

open System

type Source = TA | ATS
type Currency = PLN | EUR | USD
type Pair = Currency * Currency
type InputTick = { Ask: decimal; Bid: decimal; Pair: Pair; Source: Source; TimeStamp: DateTime }
type OutputTick = { Ask: decimal; Bid: decimal; Pair: Pair }
type Tick = Valid of OutputTick | BadTick of string
type InputStream = IObservable<InputTick>
type OutputStream = IObservable<Tick>