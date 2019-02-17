namespace RiverMeasurements.Bom

module TimeParser =

    open System
    open System.Text.RegularExpressions

    let tokenizeStr (v : string) =
        let m = Regex.Match(v, "(\d+)\.(\d+)([AaPpMm]+) (\w+)") 
        if (m.Success) then
            Some (m.Groups.[4].Value, System.Int32.Parse(m.Groups.[1].Value), System.Int32.Parse(m.Groups.[2].Value), m.Groups.[3].Value)
        else
            None

    let getDayOfWeek day =
        match day with
            | "Mon" -> DayOfWeek.Monday
            | "Tue" -> DayOfWeek.Tuesday
            | "Wed" -> DayOfWeek.Wednesday
            | "Thur" -> DayOfWeek.Thursday
            | "Fri" -> DayOfWeek.Friday
            | "Sat" -> DayOfWeek.Saturday
            | "Sun" -> DayOfWeek.Sunday
            | _ -> failwith "Unknown Day"

    let calculateDate day =
        let now = DateTime.Now
        let currentDay = now.DayOfWeek
        let diff = (int currentDay) - (int (getDayOfWeek day))
        if diff > 0 then now.AddDays(float (-1 * diff) )
        elif diff < 0 then now.AddDays(float (-1 * (7 - diff)))
        else now

    let militaryHour hour (period : string) =
        match period.ToLower() with
            | "pm" -> if hour = 12 then 12 else hour + 12
            | "am" -> if hour = 12 then 0 else hour
            | _ -> failwith (sprintf "Unknown Period of %s" period)

    let calculateDateTime (day, hour, minute, period : string) =
        let date = calculateDate day
        new DateTime(date.Year, date.Month, date.Day, militaryHour hour period, minute, 0)

    let parseTime str =
        match tokenizeStr str with
            | Some x -> Some (calculateDateTime x)
            | _ -> None