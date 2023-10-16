.code
MyProc1 proc
mov RAX, 1 ;Wynik 
calc_power:
cmp RDX, 0
je done
imul RAX, RCX
dec RDX
jmp calc_power
done:
mov RCX, RAX
ret
MyProc1 endp
end


;add RCX, RDX
;mov RAX, RCX
;ret