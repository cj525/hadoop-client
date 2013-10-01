﻿#region FreeBSD

// Copyright (c) 2013, The Tribe
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//  * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
//  * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
// TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using TechTalk.SpecFlow;

namespace _specs.Steps
{
	[Binding]
	public class RequestVerification
	{
		[Then(@"a REST request should have been submitted with the following values:")]
		public void CheckRequest(Table values)
		{
			ScenarioContext.Current.Pending();
		}

		[Then(@"a REST request should have been submitted with the correct (.+) and (.+)")]
		public void CheckRequest(string method, string url)
		{
			ScenarioContext.Current.Pending();
		}

		[Then(@"the REST request should have contained (.*) cells?")]
		public void CheckRequestCellCount(int count)
		{
			ScenarioContext.Current.Pending();
		}

		[Then(@"one of the cells in the REST request should have had the value ""(.*)""")]
		public void CheckAnyCellValue(string value)
		{
			ScenarioContext.Current.Pending();
		}

		[Then(@"one of the cells in the REST request should have had the following values:")]
		public void CheckAnyCellValues(Table values)
		{
			ScenarioContext.Current.Pending();
		}
	}
}