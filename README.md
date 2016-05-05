# ReactiveQuotationEngine

PoC for reactive quotation ticks processing.

## Installation

    .paket\paket.bootstrapper.exe
    .paket\paket.exe install

## Usage

    fsi.exe .\Script.fsx

...or just run it in Visual Studio F# REPL.

## Where to start

See example formulas for [Bad Tick](./ReactiveQuotationEngine/BadTickFormula.fs) and [Min/Max](./ReactiveQuotationEngine/MinMaxFormula.fs). Beware: they are just examples and make no (business) sense whatsoover :).

## Basic ideas

* formulas take as input sequence of ticks from available sources (e.g. TA/ATS)
* all ticks are for the same currency pair (EUR/PLN etc.)
* formulas are applied to all currency pairs which have subscription (reactive streams are "lazy")
* formula outputs Tick structure with denotates Valid (calculated) or BadTick
* formulas could be edited via gui / test console ([see example](http://www.tryfsharp.org/Create)) / generated from other representation (XML/Excel)

## TODO

It's just proof of concept but... it could be easily extended to support:

* additional input (bank position, limits etc.)
* aggregate formulas (e.g. average over last n ticks)