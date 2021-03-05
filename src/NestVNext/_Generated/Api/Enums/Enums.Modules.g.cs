// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information.
//
// ███╗   ██╗ ██████╗ ████████╗██╗ ██████╗███████╗ 
// ████╗  ██║██╔═══██╗╚══██╔══╝██║██╔════╝██╔════╝ 
// ██╔██╗ ██║██║   ██║   ██║   ██║██║     █████╗   
// ██║╚██╗██║██║   ██║   ██║   ██║██║     ██╔══╝   
// ██║ ╚████║╚██████╔╝   ██║   ██║╚██████╗███████╗ 
// ╚═╝  ╚═══╝ ╚═════╝    ╚═╝   ╚═╝ ╚═════╝╚══════╝ 
// ------------------------------------------------
//
// This file is automatically generated.
// Please do not edit these files manually.
// Run the following in the root of the repository:
//
// TODO - RUN INSTRUCTIONS
//
// ------------------------------------------------
using System.Runtime.Serialization;

namespace Nest
{
    public enum AllocationEnable
    {
        [EnumMember(Value = "all")]
        All,
        [EnumMember(Value = "primaries")]
        Primaries,
        [EnumMember(Value = "new_primaries")]
        NewPrimaries,
        [EnumMember(Value = "none")]
        None
    }

    public enum AllowRebalance
    {
        [EnumMember(Value = "always")]
        Always,
        [EnumMember(Value = "indices_primaries_active")]
        IndicesPrimariesActive,
        [EnumMember(Value = "indices_all_active")]
        IndicesAllActive
    }

    public enum FielddataLoading
    {
        [EnumMember(Value = "eager")]
        Eager,
        [EnumMember(Value = "eager_global_ordinals")]
        EagerGlobalOrdinals
    }

    public enum GeoPointFielddataFormat
    {
        [EnumMember(Value = "array")]
        Array,
        [EnumMember(Value = "doc_values")]
        DocValues,
        [EnumMember(Value = "compressed")]
        Compressed,
        [EnumMember(Value = "disabled")]
        Disabled
    }

    public enum NumericFielddataFormat
    {
        [EnumMember(Value = "array")]
        Array,
        [EnumMember(Value = "disabled")]
        Disabled
    }

    public enum RebalanceEnable
    {
        [EnumMember(Value = "all")]
        All,
        [EnumMember(Value = "primaries")]
        Primaries,
        [EnumMember(Value = "replicas")]
        Replicas,
        [EnumMember(Value = "none")]
        None
    }

    public enum ScriptLang
    {
        [EnumMember(Value = "painless")]
        Painless,
        [EnumMember(Value = "expression")]
        Expression,
        [EnumMember(Value = "mustache")]
        Mustache
    }

    public enum StringFielddataFormat
    {
        [EnumMember(Value = "paged_bytes")]
        PagedBytes,
        [EnumMember(Value = "disabled")]
        Disabled
    }
}