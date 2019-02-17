namespace RiverMeasurements.Bom

module RiverHeights =

    open System
    open System.Text.RegularExpressions
    open FSharp.Data
    open RiverMeasurements.Bom.TimeParser

    type RiverHeightMeasurement = {
        StationId : string;
        Name : string;
        Taken : DateTime Nullable;
        Height : string;
        Tendency : string;
        Crossing : string;
        FloodClass : string
    }

    type RegionMeasurements = {
        RegionName : string;
        Measurements : RiverHeightMeasurement list
    }

    let splitOn test lst =
        List.foldBack (fun el lst ->
                match lst with
                | [] -> [[el]]
                | (x::xs)::ys when not (test el x) -> (el::(x::xs))::ys
                | _ -> [el]::lst
             )  lst []

    // We get a list of tuples (header tr, [record tr])
    let groupByRegion (rows : HtmlNode list) =
        rows
        |> splitOn (fun e x -> List.length(x.Elements()) = 1)
        |> List.map (fun e -> (List.head e, List.tail e))


    let (|FirstRegexGroup|_|) pattern input =
       let m = Regex.Match(input,pattern) 
       if (m.Success) then Some m.Groups.[1].Value else None 

    let getStationId (node : HtmlNode) =
        let x = node.ToString()

        let m = Regex.Match(x, "<!-- METADATA,(\d+),.+") 
        if (m.Success) then m.Groups.[1].Value else ""  

    let getStationRow (tr : HtmlNode) =
        ( getStationId(tr.Elements().Head) ,
          tr.Descendants "td" |> Seq.map (fun n -> n.DirectInnerText()) |> Seq.toList )

    let tableRowToRegionName (tr : HtmlNode) =
        tr.Elements() |> List.head |> HtmlNode.innerText

    let tableRowsToMeasurements (tr : HtmlNode list) =
        let now = DateTime.Now

        tr
        |> List.map (fun n -> getStationRow n)
        |> List.map (fun n ->
            let data = snd n
            {
                StationId = fst n;
                Name = data.[0];
                Taken = match parseTime data.[1] with
                            | Some x -> Nullable(x)
                            | _ -> Nullable()
                Height = data.[2];
                Tendency = data.[3];
                Crossing = data.[4];
                FloodClass = data.[5];
            })

    let getRiverHeightsFromPage (page : HtmlDocument) =
        page.CssSelect("table.tabledata > tbody > tr")
            |> groupByRegion
            |> List.map (fun n ->
                    {
                        RegionName = tableRowToRegionName(fst n);
                        Measurements = tableRowsToMeasurements(snd n)
                    })
