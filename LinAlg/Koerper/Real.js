function abs(a){
if(a>0){
return a;
}
return -a;
}
function multiply(a, b)
{
	return a * b;
}
function add(a, b) {
	return a + b;
}
function smaller(a, b) {
	return a < b;
}
function bigger(a, b) {
	return a > b;
}
function mult_inv(a) {
	return (1/a);
}
function add_inv(a) 
{
	return (-a);
}
function get1() 
{
	return 1;
}
function get0() 
{
	return 0;
}
function fromstr(str){
	if(str.lastIndexOf('/')!=-1){
		return(parseFloat(str.split('/')[0]) /parseFloat(str.split('/')[1]));
	}
	return parseFloat(str)
}
function find_rational( value, maxdenom ) {
	let best = { numerator: 1, denominator: 1, error: Math.abs(value - 1) }
	if ( !maxdenom ) maxdenom = 10000;
	for ( let denominator = 1; best.error > 0 && denominator <= maxdenom; denominator++ ) {
	  let numerator = Math.round( value * denominator );
	  let error = Math.abs( value - numerator / denominator );
	  if ( error >= best.error ) continue;
	  best.numerator = numerator;
	  best.denominator = denominator;
	  best.error = error;
	}
if(numerator === 0){
return "0";
}
	return "\\frac{"+best.numerator+"}{"+best.denominator+"}";
  }
function tostr(a){
	if(decimals(a)<2){
return a.toString();}
		return find_rational(a,10000);
	}
	function decimals(a) {
		if(Math.floor(a) == a) return 0;
		return a.toString().split(".")[1].length || 0; 
	}