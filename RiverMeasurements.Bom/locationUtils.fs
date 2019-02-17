namespace RiverMeasurements.Bom

module LocationUtils =

    type Direction = North | South | East | West

    type DmsCoordinate = {
        Degrees: int;
        Minutes: int;
        Seconds: int;
        Direction: Direction;
    }

    let strToDms (str : string) =
        {
            Degrees = int(str.Substring(0, 3));
            Minutes = int(str.Substring(3, 2));
            Seconds = int(str.Substring(5, 2));
            Direction = match str.Substring(7, 1) with
                            | "N" -> North
                            | "S" -> South
                            | "E" -> East
                            | "W" -> West
                            | x -> failwithf "Unknown direction of %s" x 
        }

    let dmsToDecimal dms =
        let totalsec = float((dms.Minutes * 60) + dms.Seconds)

        match dms.Direction with
            | North | East -> float(dms.Degrees) + (totalsec / 3600.0)
            | South | West -> (float(dms.Degrees) + (totalsec / 3600.0)) * -1.0


    let dmsStrToDecimalDegrees str =
        str |> strToDms |> dmsToDecimal