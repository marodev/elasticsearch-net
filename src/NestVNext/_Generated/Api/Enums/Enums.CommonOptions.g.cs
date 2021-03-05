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
    public enum DateMathOperation
    {
        [EnumMember(Value = "+")]
        Add,
        [EnumMember(Value = "-")]
        Subtract
    }

    public enum DateMathTimeUnit
    {
        [EnumMember(Value = "s")]
        s,
        [EnumMember(Value = "m")]
        m,
        [EnumMember(Value = "h")]
        h,
        [EnumMember(Value = "d")]
        d,
        [EnumMember(Value = "w")]
        w,
        [EnumMember(Value = "M")]
        M,
        [EnumMember(Value = "y")]
        y
    }

    public enum DistanceUnit
    {
        [EnumMember(Value = "in")]
        In,
        [EnumMember(Value = "ft")]
        Ft,
        [EnumMember(Value = "yd")]
        Yd,
        [EnumMember(Value = "mi")]
        Mi,
        [EnumMember(Value = "nmi")]
        Nmi,
        [EnumMember(Value = "km")]
        Km,
        [EnumMember(Value = "m")]
        m,
        [EnumMember(Value = "cm")]
        Cm,
        [EnumMember(Value = "mm")]
        Mm
    }

    public enum GeoDistanceType
    {
        [EnumMember(Value = "arc")]
        Arc,
        [EnumMember(Value = "plane")]
        Plane
    }

    public enum GeoShapeRelation
    {
        [EnumMember(Value = "intersects")]
        Intersects,
        [EnumMember(Value = "disjoint")]
        Disjoint,
        [EnumMember(Value = "within")]
        Within,
        [EnumMember(Value = "contains")]
        Contains
    }

    public enum ShapeRelation
    {
        [EnumMember(Value = "intersects")]
        Intersects,
        [EnumMember(Value = "disjoint")]
        Disjoint,
        [EnumMember(Value = "within")]
        Within
    }

    public enum TimeUnit
    {
        [EnumMember(Value = "nanos")]
        Nanos,
        [EnumMember(Value = "micros")]
        Micros,
        [EnumMember(Value = "ms")]
        Ms,
        [EnumMember(Value = "s")]
        s,
        [EnumMember(Value = "m")]
        m,
        [EnumMember(Value = "h")]
        h,
        [EnumMember(Value = "d")]
        d
    }
}