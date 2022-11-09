#make_COM
include 'emu8086.inc'
ORG 1000h
;Variables
	area DW ?
	area DD 0
	radio DW ?
	radio DD 0
	pi DW ?
	pi DD 0
	resultado DW ?
	resultado DD 0
	a DW ?
	a DW 0
	d DW ?
	d DW 0
	altura DW ?
	altura DW 0
	x DW ?
	x DD 0
	y DW ?
	y DW 0
	i DW ?
	i DW 0
	j DW ?
	j DW 0
	k DW ?
	k DW 0
	l DW ?
	l DW 0
PRINTN "Introduzca el radio del cilindro: "
RET
DEFINE_SCAN_NUM
DEFINE_PRINT_NUM
DEFINE_PRINT_NUM_UNS
DEFINE_PRINT_STR
END
