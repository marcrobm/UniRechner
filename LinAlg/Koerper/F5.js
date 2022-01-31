var prim = 5;//this has to be a prime number
function multiply(a, b)
{
	return (a * b)%prim;
}
function add(a, b) {
	return (a + b)%prim;
}
function mult_inv(a) {
for(var i = 0;i<prim;i++){
if(((a*i)%prim)==1){
return(i);
}
}
}
function add_inv(a) 
{
for(var i = 0;i<prim;i++){
if(((a+i)%prim)==0){
return(i);
}
}
}
function get1() 
{
	return 1
}
function get0() 
{
	return 0
}
function fromstr(str){
	return Math.abs(parseInt(str))%prim
}
function tostr(a){
	return a.toString();
}