#light

module Plainion.IronDoc.FSharp.Utils

open System.Text.RegularExpressions

let SubstringAfter ( value : string ) ( sep : char ) =
    let pos = value.IndexOf (sep)
    value.Substring(pos + 1)

let NormalizeSpace (value : string) =
    Regex.Replace(value.Trim(), @"\s+", " ")
