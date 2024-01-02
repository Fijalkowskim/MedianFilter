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

    mov r10, 0                  ; Initialize row
    mov r12, rdx                ; Initialize start stripe position (bitmapWitdth)
    mov r13, rdx                ; Initialize pixel index

row_loop:
    cmp r10, r8                 ; Check if we have reached the end of the stripe
    jge end_process            ; If so, end the process

    mov r11, 0                  ; Initialize column index

column_loop:
    cmp r11, rdx                ; Check if we have reached the end of the row
    jge next_row               ; If so, move to the next row

    ; Set current pixel pointer
    mov rax, r12
    add rax, rcx

    ;jmp next_pixel
    ;jmp apply_negative

;---------------Calculate 3x3 mask---------------
    ;Point to mask array
    ;-------------------------------------top-left
    cmp r11, 0    
    je handle_top_left_edge   
    mov rbx, 0  
    sub rbx, rdx
    sub rbx, 3                  ; rbx = current - width - 3
    add rbx, rax  
    movzx r13, byte ptr [rbx]     
continue_top_left:
    movq xmm1, r13     
    ;-------------------------------------top-center
    mov rbx, 0                
    sub rbx, rdx                ; rbx = current - width
    add rbx, rax    
    movzx r13, byte ptr [rbx]    
    movq xmm2, r13
    ;-------------------------------------top-right
    cmp r11, rdx
    je handle_top_right_edge
    mov rbx, 0  
               
    sub rbx, rdx      
    add rbx, 3                  ; rbx = current - width + 3
    add rbx, rax     
    movzx r13, byte ptr [rbx]  
continue_top_right:
    movq xmm3, r13

    ;-------------------------------------middle-left
    cmp r11, 0
    je handle_middle_left_edge
    mov rbx, 0  
                  
    sub rbx, 3                  ; rbx = current - 3
    add rbx, rax  
    movzx r13, byte ptr [rbx]   
continue_middle_left:
    movq xmm4, r13
    ;-------------------------------------middle-center
    mov rbx, 0
    add rbx, rax 
    movzx r13, byte ptr [rbx]   
    movq xmm5, r13
    ;-------------------------------------middle-right
     cmp r11, rdx
    je handle_middle_right_edge
    mov rbx, 0               
    add rbx, 3                  ; rbx = current + 3
    add rbx, rax      
    movzx r13, byte ptr [rbx]  
continue_middle_right:
    movq xmm6, r13
    ;-------------------------------------bottom-left
    cmp r11, 0
    je handle_bottom_left_edge
    mov rbx, 0                 
    add rbx, rdx
    sub rbx, 3                  ; rbx = current + width - 3
    add rbx, rax 
    movzx r13, byte ptr [rbx]   
continue_bottom_left:
    movq xmm7, r13
    ;-------------------------------------bottom-center
    mov rbx, 0                
    add rbx, rdx                ; rbx = current + width
    add rbx, rax   
    movzx r13, byte ptr [rbx]    
    movq xmm8, r13
    ;-------------------------------------bottom-right
    cmp r11, rdx
    je handle_bottom_right_edge
    mov rbx, 0  
                 
    add rbx, rdx      
    add rbx, 3                  ; rbx = current + width + 3
    add rbx, rax   
    movzx r13, byte ptr [rbx]   
continue_bottom_right:
    movq xmm9, r13
    push r11
    push r12
    jmp start_sorting
;---------------/Calculate 3x3 mask---------------

; Horizontal edge hadling

handle_top_left_edge:
    mov r13, 0
    jmp continue_top_left
handle_middle_left_edge:
    mov r13, 0
    jmp continue_middle_left
handle_bottom_left_edge:
    mov r13, 0
    jmp continue_bottom_left
handle_top_right_edge:
    mov r13, 0
    jmp continue_top_right
handle_middle_right_edge:
    mov r13, 0
    jmp continue_middle_right
handle_bottom_right_edge:
    mov r13, 0
    jmp continue_bottom_right


;-------------------------------------------------SORTING-------------------------------------------------
    ; xmm - 3x3 Array
    ; r13 - loop
    ; r11 - current element 
    ; r12 - next element


start_sorting:
    ; Sort the array (simple bubble sort for small arrays)                
    mov r13, 0                   ; Outer loop counter
inner_loop:
    movq r11, xmm1        ; Load current element
    movq r12, xmm2          ; Load next element
    cmp r11, r12                ; Compare current and next element
    jbe no_swap_1               ; Jump if not greater (no swap needed)
    ; Swap elements
    movq xmm1, r12            ; Store next element at current position
    movq xmm2, r11         ; Store current element at next position
no_swap_1:
movq r11, xmm2        ; Load current element
    movq r12, xmm3          ; Load next element
    cmp r11, r12                ; Compare current and next element
    jbe no_swap_2              ; Jump if not greater (no swap needed)
    ; Swap elements
    movq xmm2, r12            ; Store next element at current position
    movq xmm3, r11         ; Store current element at next position
no_swap_2:
movq r11, xmm3        ; Load current element
    movq r12, xmm4          ; Load next element
    cmp r11, r12                ; Compare current and next element
    jbe no_swap_3               ; Jump if not greater (no swap needed)
    ; Swap elements
    movq xmm3, r12            ; Store next element at current position
    movq xmm4, r11         ; Store current element at next position
no_swap_3:
movq r11, xmm4        ; Load current element
    movq r12, xmm5          ; Load next element
    cmp r11, r12                ; Compare current and next element
    jbe no_swap_4               ; Jump if not greater (no swap needed)
    ; Swap elements
    movq xmm4, r12            ; Store next element at current position
    movq xmm5, r11         ; Store current element at next position
no_swap_4:
movq r11, xmm5        ; Load current element
    movq r12, xmm6          ; Load next element
    cmp r11, r12                ; Compare current and next element
    jbe no_swap_5               ; Jump if not greater (no swap needed)
    ; Swap elements
    movq xmm5, r12            ; Store next element at current position
    movq xmm6, r11         ; Store current element at next position
no_swap_5:
movq r11, xmm6        ; Load current element
    movq r12, xmm7          ; Load next element
    cmp r11, r12                ; Compare current and next element
    jbe no_swap_6               ; Jump if not greater (no swap needed)
    ; Swap elements
    movq xmm6, r12            ; Store next element at current position
    movq xmm7, r11         ; Store current element at next position
no_swap_6:
movq r11, xmm7        ; Load current element
    movq r12, xmm8          ; Load next element
    cmp r11, r12                ; Compare current and next element
    jbe no_swap_7               ; Jump if not greater (no swap needed)
    ; Swap elements
    movq xmm7, r12            ; Store next element at current position
    movq xmm8, r11         ; Store current element at next position
no_swap_7:
movq r11, xmm8        ; Load current element
    movq r12, xmm9          ; Load next element
    cmp r11, r12                ; Compare current and next element
    jbe no_swap_8               ; Jump if not greater (no swap needed)
    ; Swap elements
    movq xmm8, r12            ; Store next element at current position
    movq xmm9, r11         ; Store current element at next position
no_swap_8:

    add r13, 1                   ; Increment outer loop counter
    cmp r13, 8                  ; Compare with the number of elements - 1
    jl inner_loop               ; Jump if outer loop counter < 8

    ; Select the middle element from the sorted array
    movq r11, xmm4
    mov byte ptr [rax], r11b
    jmp next_pixel
    

;-------------------------------------------------/SORTING-------------------------------------------------



;--Example------------
apply_negative:
    mov r14, 255
    sub r14, [rax]

    mov byte ptr [rax], r14b

    jmp next_pixel

next_pixel:
    pop r12                     
    pop r11 
    add r11, 1                 ; Increment column index
    add r12, 1                  ; Increment pixel index
    jmp column_loop             ; Repeat for the next column
next_row:
    add r10, 1                  ; Increment row index
    jmp row_loop                ; Repeat for the next row
end_process:
    ret
AsmMedianFilter endp
end