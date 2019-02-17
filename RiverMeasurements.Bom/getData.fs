namespace RiverMeasurements.Bom

module GetData =

    open RiverMeasurements.Bom.Stations
    open RiverMeasurements.Bom.RiverHeights
    open FSharp.Data

    type Measurement = {
        Data : RiverHeightMeasurement;
        Station : Station;
    }

    type AsyncResult =
        | AsyncMeasurements of RegionMeasurements list
        | AsyncStations of Station list


    let getRiverHeightsForQldAsync = async { 
        let! page = HtmlDocument.AsyncLoad("http://www.bom.gov.au/cgi-bin/wrap_fwo.pl?IDQ60005.html")
        return getRiverHeightsFromPage page
    }

    let getRiverHeightsForQld = 
        let page = HtmlDocument.Load("http://www.bom.gov.au/cgi-bin/wrap_fwo.pl?IDQ60005.html")
        getRiverHeightsFromPage page

    let getStationsForQldAsync = async {
        let! page = HtmlDocument.AsyncLoad("http://www.bom.gov.au/qld/flood/networks/section3.shtml")
        return getStationsFromPage page
    }

    let getStationsForQld = 
        let page = HtmlDocument.Load("http://www.bom.gov.au/qld/flood/networks/section3.shtml")
        getStationsFromPage page
   

    let extractData measurements (stations : Station list) =
        measurements
            |> List.collect ( fun h -> h.Measurements )
            |> List.map ( fun h -> (h, (stations |> List.tryFind (fun e -> e.StationId = h.StationId)) ) )
            |> List.filter (fun t -> (snd t) <> None )
            |> List.map ( fun t -> { Data = fst t; Station = match snd t with | Some x -> x | _ -> failwith "OMG!" } )
            |> List.toSeq

    let getMeasurementsAsync =
        let x = async {
            let! ma = Async.StartChild getRiverHeightsForQldAsync
            let! sa = Async.StartChild getStationsForQldAsync

            let! measurements = ma
            let! stations = sa

            return extractData measurements stations
        }

        x |> Async.StartAsTask

    let getMeasurements =
        extractData getRiverHeightsForQld getStationsForQld
