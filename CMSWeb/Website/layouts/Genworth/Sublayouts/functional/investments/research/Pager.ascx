<%@ Control Language="c#" %>
<div class="filter-results-pagination InvestmentResearchPager">
	<span class="inline-form-block">
		
		Items per page&nbsp;
		<select class="perpage per-page" style="width: 50px;">
			<option value="20">20</option>
			<option value="50">50</option>
			<option value="100">100</option>
			<option value="250">250</option>
		</select>&nbsp;
		<span class="pager">
			<a href="#" class="previous quo">&lsaquo;</a>&nbsp;
			Page&nbsp;
			<span class="text-input"><input type="text" class="page page-number" size="2" /></span> of <span class="pages"></span>&nbsp;
			<a href="#" class="next quo">&rsaquo;</a>
		</span>
	</span>
</div>
