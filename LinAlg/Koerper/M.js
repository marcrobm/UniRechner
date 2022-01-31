var m = 3;      // matissen laenge
var emod = Math.pow(10,m-1); // calculate mod this
                       // funktioniert mit basis 10
function multiply(a, b)
{
	return(round(a*b));
}
function add(a, b) {
  return(round(a+b));
}
function mult_inv(a) {
	return(round(1/a));
}
function add_inv(a) 
{
	return(round(-a));
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
  return(round(parseFloat(str)));
}
function round(a){
	var e = a.toExponential(m);
  var data = e.split(/[e]/);
  var num = {
  'mat':((data[0]) % emod),
  'exp':((data[1]) % emod),
  };
  return parseFloat(num.mat+"e"+num.exp);
}
function tostr(a){
	return "["+a.toExponential(m)+"]";
}