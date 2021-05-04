/*
 * Licensed to Elasticsearch B.V. under one or more contributor
 * license agreements. See the NOTICE file distributed with
 * this work for additional information regarding copyright
 * ownership. Elasticsearch B.V. licenses this file to you under
 * the Apache License, Version 2.0 (the "License"); you may
 * not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using System.Collections.Generic;
using System.Runtime.Serialization;
using Elasticsearch.Net;

namespace Nest
{
	public class Sequence<TEvent> where TEvent : class
	{
		/// <summary>
		/// Contains events matching the query. Each object represents a matching event.
		/// </summary>
		[DataMember(Name = "events")]
		public IReadOnlyCollection<Event<TEvent>> Events { get; internal set; } = EmptyReadOnly<Event<TEvent>>.Collection;

		/// <summary>
		/// Shared field values used to constrain matches in the sequence. These are defined using the by keyword in the EQL query syntax.
		/// </summary>
		[DataMember(Name = "join_keys")]
		public IReadOnlyCollection<object> JoinKeys { get; internal set; } = EmptyReadOnly<object>.Collection;
	}
}