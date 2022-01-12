//test.asm - Test the assembler to see if it produces proper bytecode
//also tests the virtual machine to make sure everything works properly
//Stephen Marz
//18 December 2017
//COSC365 - University of Tennessee -- Knoxville

main:
	goto	begin
	push 	0x0
	push 	exit
	push
begin:
	push 	start
	neg
	ifpl	start
	nop
	nop
	ifez	start
	nop
	nop
	ifnz	start
	nop
start:
	push 	-44
	not
	mul
	push 	0xd
	add
	dup
	add
	swap
exit:
	swap
	iflt exit
	push	0xa
	push	0xabcd
	push	-1234
	push	exit
	print
	dup	2
	dump
	exit 121
