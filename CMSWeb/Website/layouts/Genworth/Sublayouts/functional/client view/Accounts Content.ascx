<%@ Control Language="c#" AutoEventWireup="true" %>
<!-- START COLLABSIBLE TABLE -->
			<table class="collapsible-table collapsible-table-open" cellpadding="0" cellspacing="0" border="0" width="100%">
				<thead>
					<tr class="collapsible-table-toggler">
						<th class="first" colspan="4">
							<span>
								<span class="toggler-arrows">
									<img class="arrow-open" src="/CMSContent/Images/collapsiblePanel_arrow_down.png" />
									<img class="arrow-closed" src="/CMSContent/Images/collapsiblePanel_arrow_right.png" />
								</span>
								<span class="new-count"><span class="new-count-value">3</span></span>
								<b>Funded</b> as of 12/11/2011
							</span>
						</th>
						<th class="last options" colspan="3">
							<div class="options-table-wrapper">
								<table cellpadding="0" cellspacing="0" width="100%" border="0">
									<tr>
										<td>
											<span class="icon customize">Customize</span>
											<div class="filter-bar-list-wrapper">
												<div class="filter-bar-list">
													<div class="filter-bar-style-top"></div>
													<div class="filter-bar-style-bottom"></div>
													<div class="filter-bar-list-header">
														<label>DataGrid Visible Columns:</label>
														<ul>
															<li class="first last">
																<input type="checkbox" id="Checkbox22" class="check-all" />
													
																<label for="Checkbox22">All</label>
																<a class="defaults" href="#">Reset to defaults</a>
															</li>
														</ul>
														<hr />
													</div>
													<div class="filter-bar-list-body">
														<ul>
															<li class="first">
																<input type="checkbox" id="Checkbox1" />
													
																<label for="Checkbox1">Item 1</label>
															</li>
															<li>
																<input type="checkbox" id="Checkbox2" />
													
																<label for="Checkbox2">Item 2</label>
															</li>
															<li>
																<input type="checkbox" id="Checkbox3" />
													
																<label for="Checkbox3">Item 3</label>
															</li>
															<li class="last">
																<input type="checkbox" id="Checkbox4" />
													
																<label for="Checkbox4">Item 4</label>
															</li>
														</ul>
														
													</div>
													<div class="filter-bar-list-footer">
														<hr />
														<span class="button"><input type="button" value="Cancel" /></span>
														<span class="button"><input type="button" value="Save" /></span>
													</div>
												</div>
											</div>
										</td>
										<td>
											<span class="icon print">Print</span>
											<div class="filter-bar-list-wrapper">
												<div class="filter-bar-list">
													<div class="filter-bar-style-top"></div>
													<div class="filter-bar-style-bottom"></div>
													<div class="filter-bar-list-header">
														<label>Print Options:</label>
													</div>
													<div class="filter-bar-list-body">
														<ul>
															<li class="first">
																<a href="#">Print this page only</a>
															</li>
															<li class="last">
																<a href="#">Print all 12 pages</a>
															</li>
														</ul>
														
													</div>
												</div>
											</div>
										</td>
										<td>
											<span class="icon download">Download</span>
											<div class="filter-bar-list-wrapper">
												<div class="filter-bar-list">
													<div class="filter-bar-style-top"></div>
													<div class="filter-bar-style-bottom"></div>
													<div class="filter-bar-list-header">
														<label>Download Options:<br />(file format)</label>
													</div>
													<div class="filter-bar-list-body">
														<ul>
															<li class="first">
																<a href="#">PDF</a>
															</li>
															<li>
																<a href="#">CSV</a>
															</li>
															<li>
																<a href="#">XLS</a>
															</li>
															<li class="last">
																<a href="#">DOC</a>
															</li>
														</ul>
														
													</div>
												</div>
											</div>
										</td>
									</tr>
								</table>
							</div>
						</th>
					</tr>
					<tr class="collapsible-table-head">
						<th class="first"><span>Account Registration</span></th>
						<th class="sortable selected"><span>Account Number</span></th>
						<th><span>Investment Solution</span></th>
						<th><span>Approach</span></th>
						<th><span>Net<br />Investment</span></th>
						<th><span>Market<br />Value</span></th>
						<th class="last"><span>% of Total</span></th>
					</tr>
				</thead>
				<tbody>
					<tr class="first">
						<td class="first"><span>Victoria Emanuelson - IRA</span></td>
						<td class="sorted"><span>1234567890</span></td>
						<td width="300px"><span>Callan, DS, Distribution, 6%, Profile 2, Mod, This is supposed to be a very long block of text.</span></td>
						<td><span>Strategic</span></td>
						<td class="currency"><span>$1,234,567.89</span></td>
						<td class="currency"><span>$987,654.32</span></td>
						<td class="last percent"><span>40%</span></td>
					</tr>
					<tr>
						<td class="first"><span>Victoria Emanuelson - IRA</span></td>
						<td class="sorted"><span>1234567890</span></td>
						<td width="300px"><span>Callan, DS, Distribution, 6%, Profile 2, Mod, This is supposed to be a very long block of text.</span></td>
						<td><span>Strategic</span></td>
						<td class="currency"><span>$1,234,567.89</span></td>
						<td class="currency"><span>$987,654.32</span></td>
						<td class="last percent"><span>40%</span></td>
					</tr>
					<tr class="last">
						<td class="first"><span>Victoria Emanuelson - IRA</span></td>
						<td class="sorted"><span>1234567890</span></td>
						<td width="300px"><span>Callan, DS, Distribution, 6%, Profile 2, Mod, This is supposed to be a very long block of text.</span></td>
						<td><span>Strategic</span></td>
						<td class="currency"><span>$1,234,567.89</span></td>
						<td class="currency"><span>$987,654.32</span></td>
						<td class="last percent"><span>40%</span></td>
					</tr>
				</tbody>
				<tfoot>
					<tr class="first collapsible-table-total">
						<td class="first" colspan="4"><span>Total Accounts</span></td>
						<td class="currency"><span>$3,705,422.74</span></td>
						<td class="currency"><span>$2,842,444.96</span></td>
						<td class="last percent"><span>100%</span></td>
					</tr>
					<tr class="last dynamic-content-display">
						<td class="first dynamic-navigation-wrapper">
							<ul class="dynamic-navigation">
								<li class="first selected"><span>Asset Class</span></li>
								<li class="last"><span>Solution Type</span></li>
							</ul>
						</td>
						<td colspan="6" class="last dynamic-content-wrapper">
							<ul class="dynamic-content">
								<li class="first selected">
									<div class="client-dynamic-content-block">
										<div class="html">
											<h3>Asset Allocation Approaches</h3>
											<p>
												Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.
											</p>
										</div>
										<div>
											<table class="asset-allocation-table" border="0" cellspacing="0" cellpadding="0">
												<tr>
													<td class="strategic">
														<label>Strategic</label>
														<span>10<sub>%</sub></span>
													</td>
													<td class="tactical-constrained">
														<label>Tactical<br />Constrained</label>
														<span>55<sub>%</sub></span>
													</td>
												</tr>
												<tr>
													<td class="tactical-unconstrained">
														<label>Tactical<br />Unconstrained</label>
														<span>18<sub>%</sub></span>
													</td>
													<td class="absolute-return">
														<label>Absolute<br />Return</label>
														<span>100<sub>%</sub></span>
													</td>
												</tr>
												<tr>
													<td class="additional-investment-solutions" colspan="2">
														<span>2<sub>%</sub></span>
														<label>Additional<br />Investment Solutions</label>
													</td>
												</tr>
											</table>
										</div>
									</div>
									<div class="clear"></div>
								</li>
								<li class="last">
									<div class="client-dynamic-content-block">
										<div class="html">
											<h3>Solutioin Type</h3>
											<p>
												Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.
											</p>
										</div>
									</div>
									<div class="clear"></div>
								</li>
							</ul>
						</td>
					</tr>
				</tfoot>
			</table>
			<!-- END COLLABSIBLE TABLE -->
			
			<!-- START COLLABSIBLE TABLE -->
			<table class="collapsible-table collapsible-table-closed" cellpadding="0" cellspacing="0" border="0" width="100%">
				<thead>
					<tr class="collapsible-table-toggler">
						<th class="first" colspan="4">
							<span>
								<span class="toggler-arrows">
									<img class="arrow-open" src="/CMSContent/Images/collapsiblePanel_arrow_down.png" />
									<img class="arrow-closed" src="/CMSContent/Images/collapsiblePanel_arrow_right.png" />
								</span>
								<span class="new-count"><span class="new-count-value">3</span></span>
								<b>Pending</b> as of 12/11/2011
							</span>
						</th>
						<th class="last options" colspan="3">
							<div class="options-table-wrapper">
								<table cellpadding="0" cellspacing="0" width="100%" border="0">
									<tr>
										<td>
											<span class="icon customize">Customize</span>
											<div class="filter-bar-list-wrapper">
												<div class="filter-bar-list">
													<div class="filter-bar-style-top"></div>
													<div class="filter-bar-style-bottom"></div>
													<div class="filter-bar-list-header">
														<label>DataGrid Visible Columns:</label>
														<ul>
															<li class="first last">
																<input type="checkbox" id="Checkbox5" class="check-all" />
													
																<label for="Checkbox22">All</label>
																<a class="defaults" href="#">Reset to defaults</a>
															</li>
														</ul>
														<hr />
													</div>
													<div class="filter-bar-list-body">
														<ul>
															<li class="first">
																<input type="checkbox" id="Checkbox6" />
													
																<label for="Checkbox1">Item 1</label>
															</li>
															<li>
																<input type="checkbox" id="Checkbox7" />
													
																<label for="Checkbox2">Item 2</label>
															</li>
															<li>
																<input type="checkbox" id="Checkbox8" />
													
																<label for="Checkbox3">Item 3</label>
															</li>
															<li class="last">
																<input type="checkbox" id="Checkbox9" />
													
																<label for="Checkbox4">Item 4</label>
															</li>
														</ul>
														
													</div>
													<div class="filter-bar-list-footer">
														<hr />
														<span class="button"><input type="button" value="Cancel" /></span>
														<span class="button"><input type="button" value="Save" /></span>
													</div>
												</div>
											</div>
										</td>
										<td>
											<span class="icon print">Print</span>
											<div class="filter-bar-list-wrapper">
												<div class="filter-bar-list">
													<div class="filter-bar-style-top"></div>
													<div class="filter-bar-style-bottom"></div>
													<div class="filter-bar-list-header">
														<label>Print Options:</label>
													</div>
													<div class="filter-bar-list-body">
														<ul>
															<li class="first">
																<a href="#">Print this page only</a>
															</li>
															<li class="last">
																<a href="#">Print all 12 pages</a>
															</li>
														</ul>
														
													</div>
												</div>
											</div>
										</td>
										<td>
											<span class="icon download">Download</span>
											<div class="filter-bar-list-wrapper">
												<div class="filter-bar-list">
													<div class="filter-bar-style-top"></div>
													<div class="filter-bar-style-bottom"></div>
													<div class="filter-bar-list-header">
														<label>Download Options:<br />(file format)</label>
													</div>
													<div class="filter-bar-list-body">
														<ul>
															<li class="first">
																<a href="#">PDF</a>
															</li>
															<li>
																<a href="#">CSV</a>
															</li>
															<li>
																<a href="#">XLS</a>
															</li>
															<li class="last">
																<a href="#">DOC</a>
															</li>
														</ul>
														
													</div>
												</div>
											</div>
										</td>
									</tr>
								</table>
							</div>
						</th>
					</tr>
					<tr class="collapsible-table-head">
						<th class="first"><span>Account Registration</span></th>
						<th class="sortable selected"><span>Account Number</span></th>
						<th><span>Investment Solution</span></th>
						<th><span>Approach</span></th>
						<th><span>Net<br />Investment</span></th>
						<th><span>Market<br />Value</span></th>
						<th class="last"><span>% of Total</span></th>
					</tr>
				</thead>
				<tbody>
					<tr class="first">
						<td class="first"><span>Victoria Emanuelson - IRA</span></td>
						<td><span>1234567890</span></td>
						<td width="300px"><span>Callan, DS, Distribution, 6%, Profile 2, Mod, This is supposed to be a very long block of text.</span></td>
						<td><span>Strategic</span></td>
						<td class="currency"><span>$1,234,567.89</span></td>
						<td class="currency"><span>$987,654.32</span></td>
						<td class="last percent"><span>40%</span></td>
					</tr>
					<tr>
						<td class="first"><span>Victoria Emanuelson - IRA</span></td>
						<td><span>1234567890</span></td>
						<td width="300px"><span>Callan, DS, Distribution, 6%, Profile 2, Mod, This is supposed to be a very long block of text.</span></td>
						<td><span>Strategic</span></td>
						<td class="currency"><span>$1,234,567.89</span></td>
						<td class="currency"><span>$987,654.32</span></td>
						<td class="last percent"><span>40%</span></td>
					</tr>
					<tr class="last">
						<td class="first"><span>Victoria Emanuelson - IRA</span></td>
						<td><span>1234567890</span></td>
						<td width="300px"><span>Callan, DS, Distribution, 6%, Profile 2, Mod, This is supposed to be a very long block of text.</span></td>
						<td><span>Strategic</span></td>
						<td class="currency"><span>$1,234,567.89</span></td>
						<td class="currency"><span>$987,654.32</span></td>
						<td class="last percent"><span>40%</span></td>
					</tr>
				</tbody>
				<tfoot>
					<tr class="first collapsible-table-total">
						<td class="first" colspan="4"><span>Total Accounts</span></td>
						<td class="currency"><span>$3,705,422.74</span></td>
						<td class="currency"><span>$2,842,444.96</span></td>
						<td class="last percent"><span>100%</span></td>
					</tr>
					<tr class="last dynamic-content-display">
						<td class="first dynamic-navigation-wrapper">
							<ul class="dynamic-navigation">
								<li class="first selected"><span>Asset Class</span></li>
								<li class="last"><span>Solution Type</span></li>
							</ul>
						</td>
						<td colspan="6" class="last dynamic-content-wrapper">
							<ul class="dynamic-content">
								<li class="first selected">
									<div class="client-dynamic-content-block">
										<div class="html">
											<h3>Asset Allocation Approaches</h3>
											<p>
												Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.
											</p>
										</div>
										<div>
											<table class="asset-allocation-table" border="0" cellspacing="0" cellpadding="0">
												<tr>
													<td class="strategic">
														<label>Strategic</label>
														<span>10<sub>%</sub></span>
													</td>
													<td class="tactical-constrained">
														<label>Tactical<br />Constrained</label>
														<span>55<sub>%</sub></span>
													</td>
												</tr>
												<tr>
													<td class="tactical-unconstrained">
														<label>Tactical<br />Unconstrained</label>
														<span>18<sub>%</sub></span>
													</td>
													<td class="absolute-return">
														<label>Absolute<br />Return</label>
														<span>100<sub>%</sub></span>
													</td>
												</tr>
												<tr>
													<td class="additional-investment-solutions" colspan="2">
														<span>2<sub>%</sub></span>
														<label>Additional<br />Investment Solutions</label>
													</td>
												</tr>
											</table>
										</div>
									</div>
									<div class="clear"></div>
								</li>
								<li class="last">
									<div class="client-dynamic-content-block">
										<div class="html">
											<h3>Solutioin Type</h3>
											<p>
												Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.
											</p>
										</div>
									</div>
									<div class="clear"></div>
								</li>
							</ul>
						</td>
					</tr>
				</tfoot>
			</table>
			<!-- END COLLABSIBLE TABLE -->
      
			<!-- START COLLABSIBLE TABLE -->
			<table class="collapsible-table collapsible-table-disabled" cellpadding="0" cellspacing="0" border="0" width="100%">
				<thead>
					<tr class="collapsible-table-toggler">
						<th class="first" colspan="4">
							<span>
								<span class="toggler-arrows">
									<img class="arrow-open" src="/CMSContent/Images/collapsiblePanel_arrow_down.png" />
									<img class="arrow-closed" src="/CMSContent/Images/collapsiblePanel_arrow_right.png" />
								</span>
								<span class="new-count"><span class="new-count-value">3</span></span>
								<b>Closed</b> as of 12/11/2011
							</span>
						</th>
						<th class="last options" colspan="3">
							<div class="options-table-wrapper">
								<table cellpadding="0" cellspacing="0" width="100%" border="0">
									<tr>
										<td>
											<span class="icon customize">Customize</span>
											<div class="filter-bar-list-wrapper">
												<div class="filter-bar-list">
													<div class="filter-bar-style-top"></div>
													<div class="filter-bar-style-bottom"></div>
													<div class="filter-bar-list-header">
														<label>DataGrid Visible Columns:</label>
														<ul>
															<li class="first last">
																<input type="checkbox" id="Checkbox10" class="check-all" />
													
																<label for="Checkbox22">All</label>
																<a class="defaults" href="#">Reset to defaults</a>
															</li>
														</ul>
														<hr />
													</div>
													<div class="filter-bar-list-body">
														<ul>
															<li class="first">
																<input type="checkbox" id="Checkbox11" />
													
																<label for="Checkbox1">Item 1</label>
															</li>
															<li>
																<input type="checkbox" id="Checkbox12" />
													
																<label for="Checkbox2">Item 2</label>
															</li>
															<li>
																<input type="checkbox" id="Checkbox13" />
													
																<label for="Checkbox3">Item 3</label>
															</li>
															<li class="last">
																<input type="checkbox" id="Checkbox14" />
													
																<label for="Checkbox4">Item 4</label>
															</li>
														</ul>
														
													</div>
													<div class="filter-bar-list-footer">
														<hr />
														<span class="button"><input type="button" value="Cancel" /></span>
														<span class="button"><input type="button" value="Save" /></span>
													</div>
												</div>
											</div>
										</td>
										<td>
											<span class="icon print">Print</span>
											<div class="filter-bar-list-wrapper">
												<div class="filter-bar-list">
													<div class="filter-bar-style-top"></div>
													<div class="filter-bar-style-bottom"></div>
													<div class="filter-bar-list-header">
														<label>Print Options:</label>
													</div>
													<div class="filter-bar-list-body">
														<ul>
															<li class="first">
																<a href="#">Print this page only</a>
															</li>
															<li class="last">
																<a href="#">Print all 12 pages</a>
															</li>
														</ul>
														
													</div>
												</div>
											</div>
										</td>
										<td>
											<span class="icon download">Download</span>
											<div class="filter-bar-list-wrapper">
												<div class="filter-bar-list">
													<div class="filter-bar-style-top"></div>
													<div class="filter-bar-style-bottom"></div>
													<div class="filter-bar-list-header">
														<label>Download Options:<br />(file format)</label>
													</div>
													<div class="filter-bar-list-body">
														<ul>
															<li class="first">
																<a href="#">PDF</a>
															</li>
															<li>
																<a href="#">CSV</a>
															</li>
															<li>
																<a href="#">XLS</a>
															</li>
															<li class="last">
																<a href="#">DOC</a>
															</li>
														</ul>
														
													</div>
												</div>
											</div>
										</td>
									</tr>
								</table>
							</div>
						</th>
					</tr>
					<tr class="collapsible-table-head">
						<th class="first"><span>Account Registration</span></th>
						<th class="sortable selected"><span>Account Number</span></th>
						<th><span>Investment Solution</span></th>
						<th><span>Approach</span></th>
						<th><span>Net<br />Investment</span></th>
						<th><span>Market<br />Value</span></th>
						<th class="last"><span>% of Total</span></th>
					</tr>
				</thead>
				<tbody>
					<tr class="first">
						<td class="first"><span>Victoria Emanuelson - IRA</span></td>
						<td><span>1234567890</span></td>
						<td width="300px"><span>Callan, DS, Distribution, 6%, Profile 2, Mod, This is supposed to be a very long block of text.</span></td>
						<td><span>Strategic</span></td>
						<td class="currency"><span>$1,234,567.89</span></td>
						<td class="currency"><span>$987,654.32</span></td>
						<td class="last percent"><span>40%</span></td>
					</tr>
					<tr>
						<td class="first"><span>Victoria Emanuelson - IRA</span></td>
						<td><span>1234567890</span></td>
						<td width="300px"><span>Callan, DS, Distribution, 6%, Profile 2, Mod, This is supposed to be a very long block of text.</span></td>
						<td><span>Strategic</span></td>
						<td class="currency"><span>$1,234,567.89</span></td>
						<td class="currency"><span>$987,654.32</span></td>
						<td class="last percent"><span>40%</span></td>
					</tr>
					<tr class="last">
						<td class="first"><span>Victoria Emanuelson - IRA</span></td>
						<td><span>1234567890</span></td>
						<td width="300px"><span>Callan, DS, Distribution, 6%, Profile 2, Mod, This is supposed to be a very long block of text.</span></td>
						<td><span>Strategic</span></td>
						<td class="currency"><span>$1,234,567.89</span></td>
						<td class="currency"><span>$987,654.32</span></td>
						<td class="last percent"><span>40%</span></td>
					</tr>
				</tbody>
				<tfoot>
					<tr class="first collapsible-table-total">
						<td class="first" colspan="4"><span>Total Accounts</span></td>
						<td class="currency"><span>$3,705,422.74</span></td>
						<td class="currency"><span>$2,842,444.96</span></td>
						<td class="last percent"><span>100%</span></td>
					</tr>
					<tr class="last dynamic-content-display">
						<td class="first dynamic-navigation-wrapper">
							<ul class="dynamic-navigation">
								<li class="first selected"><span>Asset Class</span></li>
								<li class="last"><span>Solution Type</span></li>
							</ul>
						</td>
						<td colspan="6" class="last dynamic-content-wrapper">
							<ul class="dynamic-content">
								<li class="first selected">
									<div class="client-dynamic-content-block">
										<div class="html">
											<h3>Asset Allocation Approaches</h3>
											<p>
												Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.
											</p>
										</div>
										<div>
											<table class="asset-allocation-table" border="0" cellspacing="0" cellpadding="0">
												<tr>
													<td class="strategic">
														<label>Strategic</label>
														<span>10<sub>%</sub></span>
													</td>
													<td class="tactical-constrained">
														<label>Tactical<br />Constrained</label>
														<span>55<sub>%</sub></span>
													</td>
												</tr>
												<tr>
													<td class="tactical-unconstrained">
														<label>Tactical<br />Unconstrained</label>
														<span>18<sub>%</sub></span>
													</td>
													<td class="absolute-return">
														<label>Absolute<br />Return</label>
														<span>100<sub>%</sub></span>
													</td>
												</tr>
												<tr>
													<td class="additional-investment-solutions" colspan="2">
														<span>2<sub>%</sub></span>
														<label>Additional<br />Investment Solutions</label>
													</td>
												</tr>
											</table>
										</div>
									</div>
									<div class="clear"></div>
								</li>
								<li class="last">
									<div class="client-dynamic-content-block">
										<div class="html">
											<h3>Solutioin Type</h3>
											<p>
												Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat.
											</p>
										</div>
									</div>
									<div class="clear"></div>
								</li>
							</ul>
						</td>
					</tr>
				</tfoot>
			</table>
			<!-- END COLLABSIBLE TABLE -->
            