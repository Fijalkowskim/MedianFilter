.data
    filteredMaskR DB 9 DUP(?) 
    maskArray DB 9 DUP(?)
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

    jmp apply_negative

;---------------Calculate 3x3 mask---------------
    ;lea rdi, [rsp]              ; Pointer to the array on the stack
     lea rdi, [maskArray]
    ;-------------------------------------top-left
    cmp r15, 0
    je handle_top_left_edge
    mov rdi, 0                  
    sub rdi, rdx
    sub rdi, 3                  ; rdi = current - width - 3
    add rdi, rax 
    movzx r13, byte ptr [rdi]     
continue_top_left:
    mov [rdi], r13              
    ;-------------------------------------top-center
    mov rdi, 0                
    sub rdi, rdx                ; rdi = current - width
    add rdi, rax    
    movzx r13, byte ptr [rdi]    
    mov [rdi], r13
    ;-------------------------------------top-right
    mov r14, rdx
    sub r14, 3
    cmp r15, r14
    je handle_top_right_edge
    mov rdi, 0  
               
    sub rdi, rdx      
    add rdi, 3                  ; rdi = current - width + 3
    add rdi, rax     
    movzx r13, byte ptr [rdi]  
continue_top_right:
    mov [rdi], r13
    ;-------------------------------------middle-left
    cmp r15, 0
    je handle_middle_left_edge
    mov rdi, 0  
                  
    sub rdi, 3                  ; rdi = current - 3
    add rdi, rax  
    movzx r13, byte ptr [rdi]   
continue_middle_left:
    mov [rdi], r13
    ;-------------------------------------middle-center
    movzx r13, byte ptr [rax]   
    mov [rdi], r13
    ;-------------------------------------middle-right
     mov r14, rdx
    sub r14, 3
    cmp r15, r14
    je handle_middle_right_edge
    mov rdi, 0  
              
    add rdi, 3                  ; rdi = current + 3
    add rdi, rax      
    movzx r13, byte ptr [rdi]  
continue_middle_right:
    mov [rdi], r13
    ;-------------------------------------bottom-left
    cmp r15, 0
    je handle_bottom_left_edge
    mov rdi, 0  
                   
    sub rdi, rdx
    add rdi, 3                  ; rdi = current + width - 3
    add rdi, rax 
    movzx r13, byte ptr [rdi]   
continue_bottom_left:
    mov [rdi], r13
    ;-------------------------------------bottom-center
    mov rdi, 0  
                 
    add rdi, rdx                ; rdi = current + width
    add rdi, rax   
    movzx r13, byte ptr [rdi]    
    mov [rdi], r13
    ;-------------------------------------bottom-right
    mov r14, rdx
    sub r14, 3
    cmp r15, r14
    je handle_bottom_right_edge
    mov rdi, 0  
                 
    add rdi, rdx      
    add rdi, 3                  ; rdi = current + width + 3
    add rdi, rax   
    movzx r13, byte ptr [rdi]   
continue_bottom_right:
    mov [rdi], r13

next_pixel:
    add r11, 3                  ; Increment column index
    add r12, 3                  ; Increment pixel index

    jmp column_loop             ; Repeat for the next column

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
    ; r10 - Number of elements in the array


start_sorting:
    ; Sort the array (simple bubble sort for small arrays)
    mov r10, 9                  ; Number of elements in the array
    
    mov r8, 0                   ; Outer loop counter

outer_loop:
    mov r9, 0                   ; Inner loop counter

inner_loop:
;    mov al, [rdi]              ; Load current element
;    mov bl, [rdi + 1]          ; Load next element
;    cmp al, bl                ; Compare current and next element
;    jbe no_swap                 ; Jump if not greater (no swap needed)

    ; Swap elements
    ;mov byte ptr [rdi], bl              ; Store next element at current position
    ;mov [rdi + 1], al          ; Store current element at next position

no_swap:
    add r9, 1                   ; Increment inner loop counter
;    inc rdi                 ; Move to the next element
    cmp r9, r10                 ; Compare with the number of elements
    jl inner_loop               ; Jump if inner loop counter < 9

    add r8, 1                   ; Increment outer loop counter
    cmp r8, 8                   ; Compare with the number of elements - 1
    jl outer_loop               ; Jump if outer loop counter < 8

    ; Select the middle element from the sorted array
;    mov rsi, [rsp + 4]          ; rsi = middle-left

    ; Store the result pixel in the output bitmap

;    movzx r8, byte ptr [rsp + 4]

    ;movzx r14, byte ptr[rsi]
    ;mov rax, r12                ; Set current pixel pointer
    ;mov byte ptr [rax], sil     ; Store the result pixel value
    ;inc rax
    ;mov byte ptr [rax], sil     ; Store the result pixel value
    ;inc rax
    ;mov byte ptr [rax], sil     ; Store the result pixel value

   
    
    

;-------------------------------------------------/SORTING-------------------------------------------------

; Stack: r12,r11,r10,r8
    pop r12
    pop r11
    pop r10
    pop r8

    add r11, 3                  ; Increment column index
    add r12, 3                  ; Increment pixel index

    jmp column_loop             ; Repeat for the next column



next_row:
    add r10, 1                  ; Increment row index
    jmp row_loop                ; Repeat for the next row

;--Example------------
apply_negative:
mov r9,255
    sub r9, [rax]

    mov byte ptr [rax], r9b

    inc rax
     mov r9,255
    sub r9, [rax]

    mov byte ptr [rax], r9b

    inc rax
     mov r9,255
    sub r9, [rax]

    mov byte ptr [rax], r9b

    jmp next_pixel

end_process:
    ret
AsmMedianFilter endp
end