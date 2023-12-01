
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

    ; Set current pixel pointer and index
    mov rax, r12
    add rax, rcx

    push r8
    push r10
    push r11
    push r12
    push rcx

;---------------Calculate 3x3 mask---------------
    lea rdi, [rsp]              ; Pointer to the array on the stack

    ;-------------------------------------top-left
    cmp r11, 0
    je handle_top_left_edge
    mov rsi, rax                
    sub rsi, rdx
    sub rsi, 3                  ; rsi = current - width - 3
    movzx r13, byte ptr [rsi]     
continue_top_left:
    mov [rdi], r13              
    ;-------------------------------------top-center
    mov rsi, rax                
    sub rsi, rdx                ; rsi = current - width
    movzx r13, byte ptr [rsi]    
    mov [rdi], r13
    ;-------------------------------------top-right
    mov r14, rdx
    sub r14, 3
    cmp r11, r14
    je handle_top_right_edge
    mov rsi, rax                
    sub rsi, rdx      
    add rsi, 3                  ; rsi = current - width + 3
    movzx r13, byte ptr [rsi]  
continue_top_right:
    mov [rdi], r13
    ;-------------------------------------middle-left
    cmp r11, 0
    je handle_middle_left_edge
    mov rsi, rax                
    sub rsi, 3                  ; rsi = current - 3
    movzx r13, byte ptr [rsi]   
continue_middle_left:
    mov [rdi], r13
    ;-------------------------------------middle-center
    movzx r13, byte ptr [rax]   
    mov [rdi], r13
    ;-------------------------------------middle-right
     mov r14, rdx
    sub r14, 3
    cmp r11, r14
    je handle_middle_right_edge
    mov rsi, rax                
    add rsi, 3                  ; rsi = current + 3
    movzx r13, byte ptr [rsi]  
continue_middle_right:
    mov [rdi], r13
    ;-------------------------------------bottom-left
    cmp r11, 0
    je handle_bottom_left_edge
    mov rsi, rax                
    sub rsi, rdx
    add rsi, 3                  ; rsi = current + width - 3
    movzx r13, byte ptr [rsi]   
continue_bottom_left:
    mov [rdi], r13
    ;-------------------------------------bottom-center
    mov rsi, rax                
    add rsi, rdx                ; rsi = current + width
    movzx r13, byte ptr [rsi]    
    mov [rdi], r13
    ;-------------------------------------bottom-right
    mov r14, rdx
    sub r14, 3
    cmp r11, r14
    je handle_bottom_right_edge
    mov rsi, rax                
    add rsi, rdx      
    add rsi, 3                  ; rsi = current + width + 3
    movzx r13, byte ptr [rsi]   
continue_bottom_right:
    mov [rdi], r13


    push rdx                    ; Save rdx,rax on stack 
    push rax
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
    ; rxc - Number of elements in the array
    ; rdi - 3x3 Array
    ; r8 - y
    ; r9 - x

start_sorting:
    ; Sort the array (simple bubble sort for small arrays)
    mov rcx, 9                  ; Number of elements in the array
    
    mov r8, 0                   ; Outer loop counter

outer_loop:
    mov r9, 0                   ; Inner loop counter
    mov r10, rdi                ; Pointer to the beginning of the array

inner_loop:
    mov r11, [r10]              ; Load current element
    mov r12, [r10 + 1]          ; Load next element
    cmp r11, r12                ; Compare current and next element
    jbe no_swap                 ; Jump if not greater (no swap needed)

    ; Swap elements
    mov [r10], r12              ; Store next element at current position
    mov [r10 + 1], r11          ; Store current element at next position

no_swap:
    add r9, 1                   ; Increment inner loop counter
    add r10, 1                  ; Move to the next element
    cmp r9, rcx                 ; Compare with the number of elements
    jl inner_loop               ; Jump if inner loop counter < 9

    add r8, 1                   ; Increment outer loop counter
    cmp r8, 8                   ; Compare with the number of elements - 1
    jl outer_loop               ; Jump if outer loop counter < 8

    ; Select the middle element from the sorted array
    mov rsi, [rsp + 4]          ; rsi = middle-left

    ; Store the result pixel in the output bitmap
    mov rax, r12                ; Set current pixel pointer
    mov byte ptr [rax], sil     ; Store the result pixel value
    inc rax
    mov byte ptr [rax], sil     ; Store the result pixel value


    inc rax
    mov byte ptr [rax], sil     ; Store the result pixel value

    add r11, 3                  ; Increment column index
    add r12, 3                  ; Increment pixel index

     
     
    ; Stack: rax,rdx,rcx,r12,r11,r,10,r8
    pop rax
    pop rdx
    pop rcx
    pop r12
    pop r11
    pop r10
    pop r8

;-------------------------------------------------/SORTING-------------------------------------------------


    jmp column_loop             ; Repeat for the next column



next_row:
    add r10, 1                  ; Increment row index
    jmp row_loop                ; Repeat for the next row

end_process:
    ret
AsmMedianFilter endp
end