.data
    swap0 BYTE 9 DUP (1,0,2,3,4,5,6,7,8)
    swap1 BYTE 9 DUP (0,2,1,3,4,5,6,7,8)
    swap2 BYTE 9 DUP (0,1,3,2,4,5,6,7,8)
    swap3 BYTE 9 DUP (0,1,2,4,3,5,6,7,8)
    swap4 BYTE 9 DUP (0,1,2,3,5,4,6,7,8)
    swap5 BYTE 9 DUP (0,1,2,3,4,6,5,7,8)
    swap6 BYTE 9 DUP (0,1,2,3,4,5,7,6,8)
    swap7 BYTE 9 DUP (0,1,2,3,4,5,6,8,7)
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
    mov r12, rdx               ; Initialize start stripe position (bitmapWitdth)

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
    je handle_top_left  
    mov rbx, 0  
    add rbx, rax 
    sub rbx, rdx
    sub rbx, 3                  ; rbx = current - width - 3   
    call mask_to_registers
    jmp continue_top_left
handle_top_left:
   call handle_edge
continue_top_left:
    pinsrb xmm0, r13, 0 
    pinsrb xmm1, r14, 0 
    pinsrb xmm2, r15, 0 
    ;-------------------------------------top-center
    mov rbx, 0    
    add rbx, rax 
    sub rbx, rdx                ; rbx = current - width      
    call mask_to_registers   
    pinsrb xmm0, r13, 1
    pinsrb xmm1, r14, 1
    pinsrb xmm2, r15, 1
    ;-------------------------------------top-right
    mov r13, rdx
    sub r13, 3
    cmp r11, r13
    je handle_top_right
    mov rbx, 0              
    sub rbx, rdx      
    add rbx, 3                  ; rbx = current - width + 3
    add rbx, rax     
    call mask_to_registers  
    jmp continue_top_right
handle_top_right:
   call handle_edge
continue_top_right:
    pinsrb xmm0, r13, 2 
    pinsrb xmm1, r14, 2 
    pinsrb xmm2, r15, 2 
    ;-------------------------------------middle-left
    cmp r11, 0
    je handle_middle_left
    mov rbx, 0               
    sub rbx, 3
    add rbx, rax               ; rbx = current - 3   
    call mask_to_registers
    jmp continue_middle_left
handle_middle_left:
   call handle_edge
continue_middle_left:
    pinsrb xmm0, r13, 3 
    pinsrb xmm1, r14, 3 
    pinsrb xmm2, r15, 3 
    ;-------------------------------------middle-center
    mov rbx, 0
    add rbx, rax 
    call mask_to_registers
    pinsrb xmm0, r13, 4 
    pinsrb xmm1, r14, 4 
    pinsrb xmm2, r15, 4 
    ;-------------------------------------middle-right
    mov r13, rdx
    sub r13, 3
    cmp r11, r13
je handle_middle_right
    mov rbx, 0               
    add rbx, 3                  ; rbx = current + 3
    add rbx, rax      
    call mask_to_registers
    jmp continue_middle_right
handle_middle_right:
   call handle_edge
continue_middle_right:
    pinsrb xmm0, r13, 5 
    pinsrb xmm1, r14, 5 
    pinsrb xmm2, r15, 5 
    ;-------------------------------------bottom-left
    cmp r11, 0
    je handle_bottom_left
    mov rbx, 0                 
    add rbx, rdx
    sub rbx, 3                  ; rbx = current + width - 3
    add rbx, rax 
    call mask_to_registers
    jmp continue_bottom_left
handle_bottom_left:
   call handle_edge
continue_bottom_left:
    pinsrb xmm0, r13, 6
    pinsrb xmm1, r14, 6
    pinsrb xmm2, r15, 6
    ;-------------------------------------bottom-center
    mov rbx, 0                
    add rbx, rdx                ; rbx = current + width
    add rbx, rax   
    call mask_to_registers
    pinsrb xmm0, r13, 7
    pinsrb xmm1, r14, 7
    pinsrb xmm2, r15, 7
    ;-------------------------------------bottom-right
    mov r13, rdx
    sub r13, 3
    cmp r11, r13
    je handle_bottom_right
    mov rbx, 0                  
    add rbx, rdx      
    add rbx, 3                  ; rbx = current + width + 3
    add rbx, rax   
    call mask_to_registers
    jmp continue_bottom_right
handle_bottom_right:
   call handle_edge
continue_bottom_right:
    pinsrb xmm0, r13, 8
    pinsrb xmm1, r14, 8
    pinsrb xmm2, r15, 8

    ;pextrb r9, xmm0, 0
    ;mov byte ptr [rax], r9b
    ;jmp next_pixel

    call start_sorting
    movaps xmm0, xmm1
    call start_sorting
    movaps xmm0, xmm2
    call start_sorting
    jmp next_pixel

    jmp start_sorting
;---------------/Calculate 3x3 mask---------------

;-------------------------------------------------SORTING-------------------------------------------------
    ; xmm - 3x3 Array
    ; rbx - loop
    ; r9 - current element 
    ; r13 - next element

start_sorting:
    ; Sort the array (simple bubble sort for small arrays)                
    mov rbx, 0                   ; loop counter
inner_loop:
    pextrb r9, xmm0, 0       ; Load current element
    pextrb r13, xmm0, 1           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_1               ; Jump if not greater (no swap needed)
    ; Swap elements
    movdqu  xmm3, xmmword ptr [swap0] 
    pshufb  xmm0, xmm3
no_swap_1:
    pextrb r9, xmm0, 1       ; Load current element
    pextrb r13, xmm0, 2           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_2              ; Jump if not greater (no swap needed)
    ; Swap elements
    movdqu  xmm3, xmmword ptr [swap1] 
    pshufb  xmm0, xmm3
no_swap_2:
    pextrb r9, xmm0, 2       ; Load current element
    pextrb r13, xmm0, 3           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_3               ; Jump if not greater (no swap needed)
    ; Swap elements
    movdqu  xmm3, xmmword ptr [swap2] 
    pshufb  xmm0, xmm3
no_swap_3:
    pextrb r9, xmm0, 3       ; Load current element
    pextrb r13, xmm0, 4           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_4               ; Jump if not greater (no swap needed)
    ; Swap elements
    movdqu  xmm3, xmmword ptr [swap3] 
    pshufb  xmm0, xmm3
no_swap_4:
    pextrb r9, xmm0, 4       ; Load current element
    pextrb r13, xmm0, 5          ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_5               ; Jump if not greater (no swap needed)
    ; Swap elements
    movdqu  xmm3, xmmword ptr [swap4] 
    pshufb  xmm0, xmm3
no_swap_5:
    pextrb r9, xmm0, 5      ; Load current element
    pextrb r13, xmm0, 6           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_6               ; Jump if not greater (no swap needed)
    ; Swap elements
    movdqu  xmm3, xmmword ptr [swap5] 
    pshufb  xmm0, xmm3
no_swap_6:
    pextrb r9, xmm0, 6       ; Load current element
    pextrb r13, xmm0, 7           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_7               ; Jump if not greater (no swap needed)
    ; Swap elements
    movdqu  xmm3, xmmword ptr [swap6] 
    pshufb  xmm0, xmm3
no_swap_7:
    pextrb r9, xmm0, 7       ; Load current element
    pextrb r13, xmm0, 8           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_8               ; Jump if not greater (no swap needed)
    ; Swap elements
    movdqu  xmm3, xmmword ptr [swap7] 
    pshufb  xmm0, xmm3
no_swap_8:
    add rbx, 1                   ; Increment outer loop counter
    cmp rbx, 8                  ; Compare with the number of elements - 1
    jl inner_loop               ; Jump if outer loop counter < 8

    ; Select the middle element from the sorted array
    pextrb r9, xmm0, 4
    mov byte ptr [rax], r9b
    inc rax
    ret


 
;Sorting for green               
    mov rbx, 0                   ; loop counter
inner_loop_g:
    pextrb r9, xmm1, 0       ; Load current element
    pextrb r13, xmm1, 1           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_g_1               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm1, r9, 1          ; Store next element at current position
    pinsrb xmm1, r13, 0           ; Store current element at next position
no_swap_g_1:
    pextrb r9, xmm1, 1       ; Load current element
    pextrb r13, xmm1, 2           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_g_2              ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm1, r9, 2            ; Store next element at current position
    pinsrb xmm1, r13, 1           ; Store current element at next position
no_swap_g_2:
    pextrb r9, xmm1, 2       ; Load current element
    pextrb r13, xmm1, 3           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_g_3               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm1, r9, 3            ; Store next element at current position
    pinsrb xmm1, r13, 2           ; Store current element at next position
no_swap_g_3:
    pextrb r9, xmm1, 3       ; Load current element
    pextrb r13, xmm1, 4           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_g_4               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm1, r9, 4            ; Store next element at current position
    pinsrb xmm1, r13, 3           ; Store current element at next position
no_swap_g_4:
    pextrb r9, xmm1, 4       ; Load current element
    pextrb r13, xmm1, 5          ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_g_5               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm1, r9, 5            ; Store next element at current position
    pinsrb xmm1, r13, 4           ; Store current element at next position
no_swap_g_5:
    pextrb r9, xmm1, 5      ; Load current element
    pextrb r13, xmm1, 6           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_g_6               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm1, r9, 6            ; Store next element at current position
    pinsrb xmm1, r13, 5           ; Store current element at next position
no_swap_g_6:
    pextrb r9, xmm1, 6       ; Load current element
    pextrb r13, xmm1, 7           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_g_7               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm1, r9, 7            ; Store next element at current position
    pinsrb xmm1, r13, 6           ; Store current element at next position
no_swap_g_7:
    pextrb r9, xmm1, 7       ; Load current element
    pextrb r13, xmm1, 8           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_g_8               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm1, r9, 8            ; Store next element at current position
    pinsrb xmm1, r13, 7           ; Store current element at next position
no_swap_g_8:

    add rbx, 1                   ; Increment outer loop counter
    cmp rbx, 8                  ; Compare with the number of elements - 1
    jl inner_loop_g               ; Jump if outer loop counter < 8

    ; Select the middle element from the sorted array
    pextrb r9, xmm1, 4
    inc rax
    mov byte ptr [rax], r9b

;Sorting for blue              
    mov rbx, 0                   ; loop counter
inner_loop_b:
    pextrb r9, xmm2, 0       ; Load current element
    pextrb r13, xmm2, 1           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_b_1               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm2, r9, 1          ; Store next element at current position
    pinsrb xmm2, r13, 0           ; Store current element at next position
no_swap_b_1:
    pextrb r9, xmm2, 1       ; Load current element
    pextrb r13, xmm2, 2           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_b_2              ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm2, r9, 2            ; Store next element at current position
    pinsrb xmm2, r13, 1           ; Store current element at next position
no_swap_b_2:
    pextrb r9, xmm2, 2       ; Load current element
    pextrb r13, xmm2, 3           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_b_3               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm2, r9, 3            ; Store next element at current position
    pinsrb xmm2, r13, 2           ; Store current element at next position
no_swap_b_3:
    pextrb r9, xmm2, 3       ; Load current element
    pextrb r13, xmm2, 4           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_b_4               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm2, r9, 4            ; Store next element at current position
    pinsrb xmm2, r13, 3           ; Store current element at next position
no_swap_b_4:
    pextrb r9, xmm2, 4       ; Load current element
    pextrb r13, xmm2, 5          ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_b_5               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm2, r9, 5            ; Store next element at current position
    pinsrb xmm2, r13, 4           ; Store current element at next position
no_swap_b_5:
    pextrb r9, xmm2, 5      ; Load current element
    pextrb r13, xmm2, 6           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_b_6               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm2, r9, 6            ; Store next element at current position
    pinsrb xmm2, r13, 5           ; Store current element at next position
no_swap_b_6:
    pextrb r9, xmm2, 6       ; Load current element
    pextrb r13, xmm2, 7           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_b_7               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm2, r9, 7            ; Store next element at current position
    pinsrb xmm2, r13, 6           ; Store current element at next position
no_swap_b_7:
    pextrb r9, xmm2, 7       ; Load current element
    pextrb r13, xmm2, 8           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_b_8               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm2, r9, 8            ; Store next element at current position
    pinsrb xmm2, r13, 7           ; Store current element at next position
no_swap_b_8:

    add rbx, 1                   ; Increment outer loop counter
    cmp rbx, 8                  ; Compare with the number of elements - 1
    jl inner_loop_b               ; Jump if outer loop counter < 8

    ; Select the middle element from the sorted array
    pextrb r9, xmm2, 4
    inc rax
    mov byte ptr [rax], r9b


    jmp next_pixel
    

;-------------------------------------------------/SORTING-------------------------------------------------
 


;--Example------------
apply_negative:
    mov r9, 255
    sub r9, [rax]
    mov byte ptr [rax], r9b
    jmp next_pixel

next_pixel:
    add r11, 3                 ; Increment column index
    add r12, 3                  ; Increment pixel index
    jmp column_loop             ; Repeat for the next column
next_row:
    add r10, 1                  ; Increment row index
    jmp row_loop                ; Repeat for the next row
mask_to_registers:
    mov r13b, byte ptr [rbx]  
    inc rbx
    mov r14b, byte ptr [rbx] 
    inc rbx
    mov r15b, byte ptr [rbx] 
    ret
handle_edge:
    mov r13b, 0
    mov r14b, 0
    mov r15b, 0
    ret
end_process:  
    ret
AsmMedianFilter endp
end