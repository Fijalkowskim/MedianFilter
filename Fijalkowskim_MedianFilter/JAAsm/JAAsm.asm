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
    je handle_top_left_edge   
    cmp r11, 1   
    je handle_top_left_edge
    cmp r11, 2    
    je handle_top_left_edge
    mov rbx, 0  
    add rbx, rax 
    sub rbx, rdx
    sub rbx, 3                  ; rbx = current - width - 3   
    movzx r13, byte ptr [rbx]  
continue_top_left:
    pinsrb xmm0, r13, 0  
    ;-------------------------------------top-center
    mov rbx, 0    
    add rbx, rax 
    sub rbx, rdx                ; rbx = current - width      
    movzx r13, byte ptr [rbx]   
    pinsrb xmm0, r13, 1
    ;-------------------------------------top-right
    mov r13, rdx
    sub r13, 3
    cmp r11, r13
    je handle_top_right_edge
    mov r13, rdx
    sub r13, 2
    cmp r11, r13
    je handle_top_right_edge
    mov r13, rdx
    sub r13, 1
    cmp r11, r13
    je handle_top_right_edge
    mov rbx, 0              
    sub rbx, rdx      
    add rbx, 3                  ; rbx = current - width + 3
    add rbx, rax     
    movzx r13, byte ptr [rbx]  
continue_top_right:
    pinsrb xmm0, r13, 2
    ;-------------------------------------middle-left
    cmp r11, 0
    je handle_middle_left_edge
    cmp r11, 1   
    je handle_middle_left_edge
    cmp r11, 2   
    je handle_middle_left_edge
    mov rbx, 0               
    sub rbx, 3
    add rbx, rax               ; rbx = current - 3   
    movzx r13, byte ptr [rbx]     
continue_middle_left:
    pinsrb xmm0, r13, 3
    ;-------------------------------------middle-center
    mov rbx, 0
    add rbx, rax 
    movzx r13, byte ptr [rbx] 
    pinsrb xmm0, r13, 4
    ;-------------------------------------middle-right
    mov r13, rdx
    sub r13, 3
    cmp r11, r13
    je handle_middle_right_edge
    mov r13, rdx
    sub r13, 2
    cmp r11, r13
    je handle_middle_right_edge
    mov r13, rdx
    sub r13, 1
    cmp r11, r13
    je handle_middle_right_edge
    mov rbx, 0               
    add rbx, 3                  ; rbx = current + 3
    add rbx, rax      
    movzx r13, byte ptr [rbx]  
continue_middle_right:
    pinsrb xmm0, r13, 5
    ;-------------------------------------bottom-left
    cmp r11, 0
    je handle_bottom_left_edge
    cmp r11, 1   
    je handle_bottom_left_edge
    cmp r11, 2    
    je handle_bottom_left_edge
    mov rbx, 0                 
    add rbx, rdx
    sub rbx, 3                  ; rbx = current + width - 3
    add rbx, rax 
    movzx r13, byte ptr [rbx]   
continue_bottom_left:
    pinsrb xmm0, r13, 6
    ;-------------------------------------bottom-center
    mov rbx, 0                
    add rbx, rdx                ; rbx = current + width
    add rbx, rax   
    movzx r13, byte ptr [rbx]    
    pinsrb xmm0, r13, 7
    ;-------------------------------------bottom-right
    mov r13, rdx
    sub r13, 3
    cmp r11, r13
    je handle_bottom_right_edge
    mov r13, rdx
    sub r13, 2
    cmp r11, r13
    je handle_bottom_right_edge
    mov r13, rdx
    sub r13, 1
    cmp r11, r13
    je handle_bottom_right_edge
    mov rbx, 0                  
    add rbx, rdx      
    add rbx, 3                  ; rbx = current + width + 3
    add rbx, rax   
    movzx r13, byte ptr [rbx]   
continue_bottom_right:
    pinsrb xmm0, r13, 8

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
    pinsrb xmm0, r9, 1          ; Store next element at current position
    pinsrb xmm0, r13, 0           ; Store current element at next position
no_swap_1:
    pextrb r9, xmm0, 1       ; Load current element
    pextrb r13, xmm0, 2           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_2              ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm0, r9, 2            ; Store next element at current position
    pinsrb xmm0, r13, 1           ; Store current element at next position
no_swap_2:
    pextrb r9, xmm0, 2       ; Load current element
    pextrb r13, xmm0, 3           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_3               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm0, r9, 3            ; Store next element at current position
    pinsrb xmm0, r13, 2           ; Store current element at next position
no_swap_3:
    pextrb r9, xmm0, 3       ; Load current element
    pextrb r13, xmm0, 4           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_4               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm0, r9, 4            ; Store next element at current position
    pinsrb xmm0, r13, 3           ; Store current element at next position
no_swap_4:
    pextrb r9, xmm0, 4       ; Load current element
    pextrb r13, xmm0, 5          ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_5               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm0, r9, 5            ; Store next element at current position
    pinsrb xmm0, r13, 4           ; Store current element at next position
no_swap_5:
    pextrb r9, xmm0, 5      ; Load current element
    pextrb r13, xmm0, 6           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_6               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm0, r9, 6            ; Store next element at current position
    pinsrb xmm0, r13, 5           ; Store current element at next position
no_swap_6:
    pextrb r9, xmm0, 6       ; Load current element
    pextrb r13, xmm0, 7           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_7               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm0, r9, 7            ; Store next element at current position
    pinsrb xmm0, r13, 6           ; Store current element at next position
no_swap_7:
    pextrb r9, xmm0, 7       ; Load current element
    pextrb r13, xmm0, 8           ; Load next element
    cmp r9, r13                ; Compare current and next element
    jbe no_swap_8               ; Jump if not greater (no swap needed)
    ; Swap elements
    pinsrb xmm0, r9, 8            ; Store next element at current position
    pinsrb xmm0, r13, 7           ; Store current element at next position
no_swap_8:

    add rbx, 1                   ; Increment outer loop counter
    cmp rbx, 8                  ; Compare with the number of elements - 1
    jl inner_loop               ; Jump if outer loop counter < 8

    ; Select the middle element from the sorted array
    pextrb r9, xmm0, 4
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