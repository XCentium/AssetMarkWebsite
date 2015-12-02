function SolutionFinder() {

	var self = this;

	var currentStep = 0;

	var $question1 = $('.question1');
	var $question2 = $('.question2');
	var $question3 = $('.question3');
	var $question4 = $('.question4');
	
	var questions = [$question1, $question2, $question3];

	var $currentQuestion = questions[0];
	
	var $help1 = $('.help1');
	var $help2 = $('.help2');
	var $help3 = $('.help3');
	var $help4 = $('.help4');
	
	var helps = [$help1, $help2, $help3, $help4];

	var $currentHelp = helps[0];

	var $solutionFinderDIV = $(".solutionFinder");
	var $questionsDIV = $(".questions");

	var closeQuestion = function() {
		$currentQuestion.removeClass('open');
		$currentQuestion.addClass('closed');
		$currentHelp.removeClass('open');
		$currentHelp.addClass('closed');
	};

	var openQuestion = function() {
		$currentQuestion.removeClass('closed');
		$('.next').remove();
		$currentQuestion.addClass('open');
		$currentHelp.removeClass('closed');
		$currentHelp.addClass('open');
		//showNextButton();
		
		//console.log('----------------',model);		
		if(currentStep === 1) {
			$('.question1').append('<div class=answer>$'+model.investment_amount+'</div>');
		} else if(currentStep === 2) {
			var goalPretty = model.goal.replace(/_/g, ' ');
			$('.question2').append('<div class=answer>'+goalPretty+'</div>').css('text-transform', 'capitalize');
		} else if(currentStep === 3) {
			$('.question3').append('<div class=answer>'+model.risk_profile+'</div>');
		}
		if(currentStep === 1 || currentStep === 2 || currentStep === 3) {
			$('a.start_over').css('display','block');
			
		} 
		
		if(model.goal === null || model.goal === '') {
			$('.question2 .next').remove();
		}
		
		if(model.investment_amount === '25k' || model.investment_amount === '100k') {
			$('.goal_one .preserve_wealth').css('background-image','url("/cmscontent/gps/img/solution-finder/goal_bg_grey.png")');
			$('.goal_one .preserve_wealth').css('cursor','default');
		}

		// 25k conditions
		if(model.investment_amount === '25k' && model.goal === 'enhance_income') {
			$('.risk1').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk5').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk1').css('cursor','default');
			$('.risk5').css('cursor','default');
		} else if(model.investment_amount === '25k' && model.goal === 'generate_income') {
			$('.risk1').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk5').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk1').css('cursor','default');
			$('.risk5').css('cursor','default');
		}

		// 100k conditions
		if(model.investment_amount === '100k' && model.goal === 'enhance_income') {
			$('.risk1').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk5').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk1').css('cursor','default');
			$('.risk5').css('cursor','default');
		} else	if(model.investment_amount === '100k' && model.goal === 'generate_income') {
			$('.risk1').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk5').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk1').css('cursor','default');
			$('.risk5').css('cursor','default');
		} else if(model.investment_amount === '25k' && model.goal === 'generate_income') {
			$('.risk1').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk5').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk1').css('cursor','default');
			$('.risk5').css('cursor','default');
		}

		// 250k conditions
		if(model.investment_amount === '250k' && model.goal === 'enhance_income') {
			$('.risk1').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk5').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk1').css('cursor','default');
			$('.risk5').css('cursor','default');
		} else if(model.investment_amount === '250k' && model.goal === 'generate_income') {
			$('.risk1').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk5').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk1').css('cursor','default');
			$('.risk5').css('cursor','default');
		} else if(model.investment_amount === '250k' && model.goal === 'preserve_wealth') {
			$('.risk2').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk3').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk4').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk5').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk2').css('cursor','default');
			$('.risk3').css('cursor','default');
			$('.risk4').css('cursor','default');
			$('.risk5').css('cursor','default');
		}

		// 1MM+ conditions
		if(model.investment_amount === '1MM+' && model.goal === 'enhance_income') {
			$('.risk1').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk5').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk1').css('cursor','default');
			$('.risk5').css('cursor','default');
		} else if(model.investment_amount === '1MM+' && model.goal === 'generate_income') {
			$('.risk1').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk5').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk1').css('cursor','default');
			$('.risk5').css('cursor','default');
		} else if(model.investment_amount === '1MM+' && model.goal === 'preserve_wealth') {
			$('.risk2').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk3').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk4').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk5').css('background-image','url("/cmscontent/gps/img/solution-finder/risk_bg_grey.png")');
			$('.risk2').css('cursor','default');
			$('.risk3').css('cursor','default');
			$('.risk4').css('cursor','default');
			$('.risk5').css('cursor','default');
		}
		
	};

	this.showNextQuestion = function() {
		currentStep += 1;

		if (currentStep === questions.length || currentStep === 4) {
			currentStep = 0;
			showResults();
			$('.next').remove();
			return;
		}

		closeQuestion(self.$currentQuestion);

		$currentQuestion = questions[currentStep];
		$currentHelp = helps[currentStep];

		openQuestion($currentQuestion);
	};

	$questionsDIV.click(function(e) {
		e.preventDefault();
		
		var hash = e.target.hash;

		if (hash === '#next') {
			self.showNextQuestion();
		}

	});

	$question2.click(function(e) {
		if($(this).hasClass('closed')) {
			return;
		}
		e.preventDefault();
		
		
		
		var hash = e.target.hash;
		
		if (hash === '#grow_wealth') {
			model.goal = 'grow_wealth';
		}
		else if (hash === '#generate_income') {
			model.goal = 'generate_income';
		}
		else if (hash === '#preserve_wealth') {
			model.goal = 'preserve_wealth';
		}
		else if (hash === '#reduce_volatility') {
			model.goal = 'reduce_volatility';
		}
		else if (hash === '#enhance_income') {
			model.goal = 'enhance_income';
		}
		else if (hash === '#reduce_downside_risk') {
			model.goal = 'reduce_downside_risk';
		}
		
		
		if(e.target.hash !== undefined) {
			$('.question2 .goal_one a').removeClass('active');
			$('.question2 .goal_two a').removeClass('active');
			$(e.target).addClass('active');
		}
		

		if(model.goal === '' || model.goal === null) {
			$('.question2 .next').remove();
		} else {
			if(hash === '#preserve_wealth') {		
				if(model.investment_amount === '250k' || model.investment_amount === '1MM+') {
					//console.log('hash? '+model.investment_amount);
					showNextButton();
				} else {
					$('.question2 .next').remove();
				}
			} else {
				showNextButton();
			}
		}

	});

	$question3.click(function(e) {
		if($(this).hasClass('closed')) {
			return;
		}
		e.preventDefault();

		var hash = e.target.hash;
		
		if (hash === '#1') {
			model.risk_profile = 1;
		}
		else if (hash === '#2') {
			model.risk_profile = 2;
		}
		else if (hash === '#3') {
			model.risk_profile = 3;
		}
		else if (hash === '#4') {
			model.risk_profile = 4;
		}
		else if (hash === '#5') {
			model.risk_profile = 5;
		}
		$('.risks a.risk').removeClass('active');
		$(e.target).addClass('active');
		
		if(model.goal === 'generate_income' || model.goal === 'enhance_income') {	
			if(hash === '#1' || hash === '#5') {	
				$('.question3 .next').remove();
			} else {
				showNextButton();
				
			}
		} else if(model.goal === 'preserve_wealth') {
			if(hash === '#2' || hash === '#3' || hash === '#4' || hash === '#5') {
				$('.question3 .next').remove();
			} else {
				showNextButton();
			}
		} else {
			showNextButton();
		}
		
		if (model.investment_amount === '250k' || model.investment_amount === '1MM+') {
			if(model.goal === 'grow_wealth' && model.risk_profile !== 1) {
				questions.push($question4);
				$('.solutionFinder .question4').css('display', 'block');
			}
		} else {
			$('div.question3 a.next').addClass('next_wide');
			$('div.question3 a.next').html('Show Results');
		}
	});

	$question4.click(function(e) {
		if($(this).hasClass('closed')) {
			return;
		}
		e.preventDefault();

		var hash = e.target.hash;

		if (hash === '#active_mutual_funds') {
			model.strategy_tilt = 'active_mutual_funds';
		}
		else if (hash === '#etf_cost_awareness') {
			model.strategy_tilt = 'etf_cost_awareness';
		}
		
		showNextButton();
		
		$('div.question4 a.next').addClass('next_wide');
		$('div.question4 a.next').html('Show Results');
		
		$('.tilts a.tilt').removeClass('active');
		$(e.target).addClass('active');
	});

	var model = {
		investment_amount: '',
		goal: '',
		risk_profile: '',
		strategy_tilt: ''
	};

	this.model = model; // for dev debug

	var showNextButton = function() {
		$('.next').remove();
		$currentQuestion.append('<a href=#next class=next>Next</a>');
	};

	var showResults = function() {
		var plans = map(),
			data = getPlanData(plans),
			i, len = data.length,
			product, mandate;
			
		$('.results_container ul').empty();
		$('.questions').hide();
		$('a.start_over').hide();
		$('.solutionFinder h2.question').hide();
		$('.results_container').show();
		$('.results').show();
		$('.client_info').show();
		
		$('.client_info .amount p').html('$'+model.investment_amount);
		var goalPretty = model.goal.replace(/_/g, ' ');
		$('.client_info .goal p').html(goalPretty).css('text-transform', 'capitalize');
		$('.client_info .risk p').html(model.risk_profile);
		
		if (model.investment_amount === '250k' || model.investment_amount === '1MM+') {
			if(model.goal === 'grow_wealth' && model.risk_profile !== 1) {
				if(model.strategy_tilt === 'etf_cost_awareness') {
					var tiltPretty = 'ETFs & Cost Awareness ';
				} else {
					var tiltPretty = 'Active Mutual Funds';
				}
			}
			
			$('.client_info .tilt p').html(tiltPretty);
			$('.client_info .tilt').css('display','block')
		}

		for (i=0; i<len; i++) {
			product = data[i];
			var productName = product.name;
			$('.results_container ul').append('' +
				'<li><a href="#tabs-' + i + '">' + productName + '</a></li>'
			);
			
			if(productName === 'GPS Tactical Unconstrained and Absolute Return' && model.risk_profile === 1) {
				mandate = 'Focused Absolute Return';
			} else if(productName === 'GPS Tactical Unconstrained and Absolute Return' && model.risk_profile === 5) {
				mandate = 'Focused Unconstrained Return';
			} else {
				mandate = product.mandate;
			}

			$('.results_container').append('' +
				'<div id="tabs-' + i + '" class="product_result">' +
				'<h5>Description</h5>' +
				'<p>' + product.description + '</p>' +
				'<h4>' + product.asset_allocation_heading + '</h4>' +
				'<img src="/~/media/Assetmark/Images/GPS/solution-finder/' + productName.replace(/ /g, '_') + '_' + model.risk_profile + '.gif" />' +
				'<p class="legal">' + product.disclaimer + '</p>'+
				'<h4>eWealthManager Account Wizard: Portfolio Construction Details</h4>' +
				'<table cellpadding=0 cellspacing=0>' +
					'<caption>Accounts Section</caption>' +
					'<tr>' +
						'<th>Portfolio Template</th>' +
						'<td>Custom</td>' +
					'</tr>' +
					'<tr>' +
						'<th>Asset Allocation Approach</th>' +
						'<td>' + product.asset_allocation_approach + '</td>' +
					'</tr>' +
					'<tr>' +
						'<th>Investment Solution</th>' +
						'<td>' + product.investment_solution + '</td>' +
					'</tr>' +
					'<tr>' +
						'<th>Solution Type</th>' +
						'<td>' + product.solution_type + '</td>' +
					'</tr>' +
					'<tr>' +
						'<th>Portfolio Strategist</th>' +
						'<td>' + product.portfolio_strategist + '</td>' +
					'</tr>' +
					'<tr>' +
						'<th>Mandate</th>' +
						'<td>' + mandate + '</td>' +
					'</tr>' +
					'<tr>' +
						'<th>Risk/Return Profile</th>' +
						'<td>' + model.risk_profile + '</td>' +
					'</tr>' +
					'<tr>' +
						'<th>Amount</th>' +
						'<td>' + model.investment_amount + '</td>' +
					'</tr>' +
				'</table>' +
				'<a class="details" href="' + product.link + '">View solution details &gt;</a> <a class="print" href="javascript:window.print()">PRINT RESULTS</a>' +
			'</div>'
			);
		}
		
		$('.results_container').tabs();
	};
	
	this.showResults = showResults; // for dev debug

	var moveAmountBubble = function(e) {
		//console.log('--------amount = '+$('a.ui-slider-handle').css('left'));
		$('#amount').css('left',$('a.ui-slider-handle').css('left'));
	};
	
	var investmentAmountArray = ["25k","100k","250k","1MM+"];

	showNextButton();
	$("#slider-range-min" ).slider({
		min: 0,
		value: 0,
		max: investmentAmountArray.length - 1,
		step:1,
		slide: function( event, ui ) {
			$( "#amount" ).val( investmentAmountArray[ui.value]);
			model.investment_amount = investmentAmountArray[ui.value];	
		}, 
		change: function(event,ui) {
			$( "#amount" ).val( investmentAmountArray[ui.value]);
			model.investment_amount = investmentAmountArray[ui.value];	
			$('#amount').css('left',$('a.ui-slider-handle').css('left'));	
		}
	});
	
	if(model.investment_amount === '') {
		model.investment_amount = '25k';
	}

	$( "#amount" ).val(investmentAmountArray[0]);

	$(".ui-slider-handle").mousedown(function(e) {
		$solutionFinderDIV.mousemove(moveAmountBubble);
	});

	$question1.mouseup(function(e) {
		$solutionFinderDIV.unbind('mousemove', moveAmountBubble);
	});

	$('.ui-slider-handle').mouseup(function(e) {
		//model.investment_amount = $( "#slider-range-min").slider( "value" ); 
	});
	

	var map = function() {
		var investment_amount = model.investment_amount,
			goal = model.goal,
			risk_profile = model.risk_profile,
			strategy_tilt = model.strategy_tilt;
			
		if(investment_amount === '25k') {
			investment_amount = 25;
		} else if(investment_amount === '100k') {
			investment_amount = 100;
		} else if(investment_amount === '250k') {
			investment_amount = 250;
		} else if(investment_amount === '1MM+') {
			investment_amount = 250; // results for 1m+ are the same as 250
		}

		if (investment_amount >= 250 && goal === 'grow_wealth' && risk_profile === 1) {
			return ['GPS Accumulation of Wealth'];
		}
		else if (investment_amount >= 250 
				&& goal === 'grow_wealth' 
				&& (risk_profile === 2 ||
					risk_profile === 3 || 
					risk_profile === 4 ||
					risk_profile === 5 )
				&& strategy_tilt === 'etf_cost_awareness') {
			return ['GPS Select Accumulation', 'GPS Accumulation of Wealth'];
		}
		else if (investment_amount >= 250 
				&& goal === 'grow_wealth' 
				&& (risk_profile === 2 ||
					risk_profile === 3 || 
					risk_profile === 4 ||
					risk_profile === 5 )
				&& strategy_tilt === 'active_mutual_funds') {
			return ['GPS Select Accumulation Plus', 'GPS Accumulation of Wealth'];
		}
		else if (investment_amount <= 250 
				&& goal === 'grow_wealth' 
				&& (risk_profile === 1 ||
					risk_profile === 2 || 
					risk_profile === 3 || 
					risk_profile === 4 ||
					risk_profile === 5 )) {
			return ['GPS Accumulation of Wealth'];
		}
		else if (investment_amount >= 250 
				&& goal === 'generate_income' 
				&& (risk_profile === 2 ||
					risk_profile === 3 || 
					risk_profile === 4 )) {
			return ['GPS Select Retirement Income', 'GPS Distribution of Wealth'];
		}
		else if (investment_amount <= 250 
				&& goal === 'generate_income' 
				&& (risk_profile === 2 ||
					risk_profile === 3 || 
					risk_profile === 4 )) {
			return ['GPS Distribution of Wealth'];
		}
		else if (investment_amount >= 250 
				&& goal === 'preserve_wealth' 
				&& risk_profile === 1) {
			return ['GPS Select Wealth Preservation'];
		}
		else if (investment_amount <= 250 
				&& goal === 'preserve_wealth') {
			return ['No options for range...'];
		}
		else if (investment_amount >= 100 
				&& goal === 'enhance_income' 
				&& (risk_profile === 2 ||
					risk_profile === 3 || 
					risk_profile === 4 )) {
			return ['GPS Select Multi-Asset Income', 'GPS Multi-Asset Income'];
		}
		else if (investment_amount <= 100 
				&& goal === 'enhance_income' 
				&& (risk_profile === 2 ||
					risk_profile === 3 || 
					risk_profile === 4 )) {
			return ['GPS Multi-Asset Income'];
		}
		else if (investment_amount >= 100 
				&& goal === 'reduce_volatility' 
				&& risk_profile === 1) {
			return ['GPS Select Low Volatility', 'GPS Tactical Unconstrained and Absolute Return'];
		}
		else if (investment_amount >= 100 
				&& goal === 'reduce_volatility' 
				&& (risk_profile === 2 ||
					risk_profile === 3 || 
					risk_profile === 4 || 
					risk_profile === 5 )) {
			return ['GPS Tactical Unconstrained and Absolute Return'];
		}
		else if (investment_amount <= 100 
				&& goal === 'reduce_volatility' 
				&& (risk_profile === 1 ||
					risk_profile === 2 || 
					risk_profile === 3 || 
					risk_profile === 4 || 
					risk_profile === 5 )) {
			return ['GPS Tactical Unconstrained and Absolute Return'];
		}
		else if (investment_amount >= 100 
				&& goal === 'reduce_downside_risk' 
				&& risk_profile === 1) {
			return ['GPS Tactical Unconstrained and Absolute Return'];
		}
		else if (investment_amount >= 100 
				&& goal === 'reduce_downside_risk' 
				&& (risk_profile === 2 ||
					risk_profile === 3 || 
					risk_profile === 4 || 
					risk_profile === 5 )) {
			return ['GPS Select Tactical Advantage', 'GPS Tactical Unconstrained and Absolute Return'];
		}
		else if (investment_amount <= 100 
				&& goal === 'reduce_downside_risk' 
				&& (risk_profile === 1 ||
					risk_profile === 2 || 
					risk_profile === 3 || 
					risk_profile === 4 || 
					risk_profile === 5 )) {
			return ['GPS Tactical Unconstrained and Absolute Return'];
		}
		else {
			return []; 
		}
	};

	this.map = map;

	var plan_data = {
		'GPS Select Accumulation': {
			name: 'GPS Select Accumulation',
			description: 'A comprehensive, diversified investment solution emphasizing ETFs and cost awareness for an investor who is accumulating assets for the future.',
			asset_allocation_heading: 'Asset Allocation Approaches by Strategist',
			asset_allocation_approach: 'Guided Portfolios',
			investment_solution: 'GPS Select Solutions',
			solution_type: 'Multi Strategy',
			portfolio_strategist: 'AssetMark',
			mandate: 'Select Accumulation',
			profiles: '2-5',
			link: '/en/gps/gps-select-solutions/accumulation.aspx',
			disclaimer:''
		},
		'GPS Select Accumulation Plus': {
			name: 'GPS Select Accumulation Plus',
			description: 'A comprehensive, diversified investment solution emphasizing active mutual funds for an investor who is accumulating assets for the future.',
			asset_allocation_heading: 'Asset Allocation Approaches by Strategist',
			asset_allocation_approach: 'Guided Portfolios',
			investment_solution: 'GPS Select Solutions',
			solution_type: 'Multi Strategy',
			portfolio_strategist: 'AssetMark',
			mandate: 'Select Accumulation Plus',
			profiles: '2-5',
			link: '/en/gps/gps-select-solutions/accumulation-plus.aspx',
			disclaimer:''
		},
		'GPS Select Retirement Income': {
			name: 'GPS Select Retirement Income',
			description: 'A comprehensive, diversified investment solution designed for an investor who is seeking to generate income and mitigate portfolio volatility.',
			asset_allocation_heading: 'Asset Allocation Approaches by Strategist',
			asset_allocation_approach: 'Guided Portfolios',
			investment_solution: 'GPS Select Solutions',
			solution_type: 'Multi Strategy',
			portfolio_strategist: 'AssetMark',
			mandate: 'Select Retirement Income',
			profiles: '2-4',
			link: '/en/gps/gps-select-solutions/retirement-income.aspx',
			disclaimer:'Multi-asset income strategies comprise 100% of the Strategic, Tactical Constrained and Tactical Unconstrained portions of the portfolio.'
		},
		'GPS Select Wealth Preservation': {
			name: 'GPS Select Wealth Preservation',
			description: 'A comprehensive, diversified investment solution designed for an investor who prioritizes wealth preservation and protection from inflation.',
			asset_allocation_heading: 'Asset Allocation Approaches by Strategist',
			asset_allocation_approach: 'Guided Portfolios',
			investment_solution: 'GPS Select Solutions',
			solution_type: 'Multi Strategy',
			portfolio_strategist: 'AssetMark',
			mandate: 'Select Wealth Preservation',
			profiles: '1',
			link: '/en/gps/gps-select-solutions/wealth-preservation.aspx',
			disclaimer:''
		},
		'GPS Select Tactical Advantage': {
			name: 'GPS Select Tactical Advantage',
			description: 'A focused investment solution with the goal of increasing portfolio diversification through less correlated strategies and capturing opportunities across evolving markets.',
			asset_allocation_heading: 'Asset Allocation Approaches by Strategist',
			asset_allocation_approach: 'Guided Portfolios',
			investment_solution: 'GPS Select Solutions',
			solution_type: 'Multi Strategy',
			portfolio_strategist: 'AssetMark',
			mandate: 'Select Tactical Advantage',
			profiles: '2-5',
			link: '/en/gps/gps-select-solutions/tactical-advantage.aspx',
			disclaimer:''
		},
		'GPS Select Low Volatility': {
			name: 'GPS Select Low Volatility',
			description: 'A focused investment solution targeting low volatility and capital preservation.',
			asset_allocation_heading: 'Asset Allocation Approaches by Strategist',
			asset_allocation_approach: 'Guided Portfolios',
			investment_solution: 'GPS Select Solutions',
			solution_type: 'Multi Strategy',
			portfolio_strategist: 'AssetMark',
			mandate: 'Select Low Volatility',
			profiles: '1',
			link: '/en/gps/gps-select-solutions/low-volatility.aspx',
			disclaimer:''
		},
		'GPS Select Multi-Asset Income': {
			name: 'GPS Select Multi-Asset Income',
			description: 'A focused investment solution with the goal of providing an enhanced and sustainable level of income across changing markets.',
			asset_allocation_heading: 'Asset Allocation Approaches by Strategist',
			asset_allocation_approach: 'Guided Portfolios',
			investment_solution: 'GPS Select Solutions',
			solution_type: 'Multi Strategy',
			portfolio_strategist: 'AssetMark',
			mandate: 'Select Multi-Asset Income',
			profiles: '2-4',
			link: '/en/gps/gps-select-solutions/multi-asset-income.aspx',
			disclaimer:'Multi-asset income strategies comprise 100% of the Strategic, Tactical Constrained and Tactical Unconstrained portions of the portfolio.'
		},
		'GPS Accumulation of Wealth': {
			name: 'GPS Accumulation',
			description: 'A comprehensive investment solution with balanced and diversified exposure for an investor who is accumulating assets for the future.',
			asset_allocation_heading: 'Asset Allocation Approaches',
			asset_allocation_approach: 'Guided Portfolios',
			investment_solution: 'GPS Solutions',
			solution_type: 'GuidePath Funds',
			portfolio_strategist: 'GPS Solutions',
			mandate: 'Accumulation - Neutral',
			profiles: '1-5',
			link: '/en/gps/gps-solutions/accumulation.aspx',
			disclaimer:''
		},
		'GPS Distribution of Wealth': {
			name: 'GPS Distribution',
			description: 'A comprehensive investment solution with a goal of generating income and distributing wealth while limiting the impact of market volatility',
			asset_allocation_heading: 'Asset Allocation Approaches',
			asset_allocation_approach: 'Guided Portfolios',
			investment_solution: 'GPS Solutions',
			solution_type: 'GuidePath Funds',
			portfolio_strategist: 'GPS Solutions',
			mandate: 'Distribution - Neutral',
			profiles: '2-4',
			link: '/en/gps/gps-solutions/distribution.aspx',
			disclaimer:'Multi-asset income strategies comprise 50% of the Strategic, Tactical Constrained and Tactical Unconstrained portions of the portfolio.'
		},
		'GPS Tactical Unconstrained and Absolute Return': {
			name: 'GPS Tactical Unconstrained and Absolute Return',
			description: 'A focused investment solution that aims to reduce downside risk and capture opportunities across evolving markets.',
			asset_allocation_heading: 'Asset Allocation Approaches',
			asset_allocation_approach: 'Guided Portfolios',
			investment_solution: 'GPS Solutions',
			solution_type: 'GuidePath Funds',
			portfolio_strategist: 'GPS Solutions',
			mandate: 'Focused Unconstrained / Absolute Return',
			profiles: '2-4',
			link: '/en/gps/gps-solutions/tactical.aspx',
			disclaimer:''
		},
		'GPS Multi-Asset Income': {
			name: 'GPS Multi-Asset Income',
			description: 'A focused investment solution that aims to enhance portfolio income across changing markets.',
			asset_allocation_heading: 'Asset Allocation Approaches',
			asset_allocation_approach: 'Guided Portfolios',
			investment_solution: 'GPS Solutions',
			solution_type: 'GuidePath Funds',
			portfolio_strategist: 'GPS Solutions',
			mandate: 'Focused Multi-Asset Income',
			profiles: '2-4',
			link: '/en/gps/gps-solutions/multi-asset.aspx',
			disclaimer:'Multi-asset income strategies comprise 60-100% of the Strategic, Tactical Constrained and Tactical Unconstrained portions of the portfolio.'
		},
		'GPS Absolute Return': {
			name: 'GPS Absolute Return',
			description: 'A focused investment solution that aims to reduce downside risk and capture opportunities across evolving markets.',
			asset_allocation_heading: 'Asset Allocation Approaches',
			asset_allocation_approach: 'Guided Portfolios',
			investment_solution: 'GPS Solutions',
			solution_type: 'GuidePath Funds',
			portfolio_strategist: 'GPS Solutions',
			mandate: 'Focused Absolute Returns',
			profiles: '1',
			link: '/en/gps/gps-solutions/tactical.aspx',
			disclaimer:''
		},
		'GPS Tactical Unconstrained': {
			name: 'GPS Tactical Unconstrained',
			description: 'A focused investment solution that aims to reduce downside risk and capture opportunities across evolving markets.',
			asset_allocation_heading: 'Asset Allocation Approaches',
			asset_allocation_approach: 'Guided Portfolios',
			investment_solution: 'GPS Solutions',
			solution_type: 'GuidePath Funds',
			portfolio_strategist: 'GPS Solutions',
			mandate: 'Focused Unconstrained Returns',
			profiles: '5',
			link: '/en/gps/gps-solutions/tactical.aspx',
			disclaimer:''
		}
	};

	var getPlanData = function(plans) {
		if (plans === undefined) {
			return [];
		}

		var i, 
			len=plans.length,
			datas = [];

		for (i=0; i<len; i++) {
			datas.push(plan_data[plans[i]]);
		}
		return datas;
	}

	this.getPlanData = getPlanData; // for dev depug

}


$(function() {
	var solutionFinder = new SolutionFinder();

	window.sf = solutionFinder;
	
	$('dt.one').click(function() {
		$(this).toggleClass('close');
		$('.goal_one').toggleClass('close');
	});
	
	$('dt.two').click(function() {
		$(this).toggleClass('close');
		$('.goal_two').toggleClass('close');
	});

});
