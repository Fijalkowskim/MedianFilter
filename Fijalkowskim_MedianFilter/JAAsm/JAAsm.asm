.code
AsmMedianFilter proc
    ; Parameters:
    ; rcx - bitmap stripe 
    ; rdx - bitmap width
    ; r8 - number of rows in this stripe
    ; r10 - y (row)
    ; r11 - x (column)
    ; r12 - current pixel index
    ; rax - current pixel pointer

    mov r10, 0          ; Initialize row
    mov r12, rdx        ; Initialize start stripe position (bitmapWitdth)

row_loop:
    cmp r10, r8        ; Check if we have reached the end of the stirpe
    jge end_process    ; If so, end the process

    mov r11, 0          ; Initialize column index

column_loop:
    cmp r11, rdx        ; Check if we have reached the end of the row
    jge next_row       ; If so, move to the next row

    ; Add pixel processing code at position [rdi + (r8 * rdx + r9) * 3]

    mov rax, r12        
    add rax, rcx        ; Set current pixel pointer

    ; Modify the pixel
    mov bl, 255
    sub bl, byte ptr [rax]
    mov byte ptr [rax], bl
    inc rax
    mov bl, 255
    sub bl, byte ptr [rax]
    mov byte ptr [rax], bl
    inc rax
    mov bl, 255
    sub bl, byte ptr [rax]
    mov byte ptr [rax], bl

    add r12, 3          ; Current pixel index += 3

    add r11, 3          ; Increment column index
    jmp column_loop    ; Repeat for the next column

next_row:
    add r10, 1          ; Increment row index
    jmp row_loop       ; Repeat for the next row

end_process:
    ret
AsmMedianFilter endp
end