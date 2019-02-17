namespace RiverMeasurements.Bom

module Stations =

    open System
    open System.Text.RegularExpressions
    open FSharp.Data
    open RiverMeasurements.Bom.LocationUtils

    type Station = {
        StationId : string;
        Name : string;
        Latitude : float;
        Longitude : float;
    }

    let trimStr (str : String) =
        str.Trim()

    // 039319   136101 ABERCORN TM                    THREE MOON CREEK        BURNETT              250745S   1510750E    
    let lineToStation (str : String) =
        try
            Some {
                StationId = str.Substring(1, 6);
                Name = str.Substring(17, 31).Trim();
                Latitude = dmsStrToDecimalDegrees(str.Substring(92, 8));
                Longitude = dmsStrToDecimalDegrees(str.Substring(103, 8));
            }
        with
            | _ -> None

    let contentToStations (str : String) =
        let regex = new Regex("^ \d+")

        str.Split('\n')
        |> Array.filter (fun e -> regex.IsMatch(e) )
        |> Array.map (fun e -> lineToStation e)
        |> Array.filter (fun opt -> opt <> None)
        |> Array.choose id
        |> Array.toList

    let getStationsFromPage (page : HtmlDocument) =
        page.CssSelect("div#content > pre")
            |> List.head
            |> HtmlNode.directInnerText
            |> trimStr
            |> contentToStations
