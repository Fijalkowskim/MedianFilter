.data
    maskArraySize equ 9
    maxThreads equ 64
    maskArray QWORD maxThreads * maskArraySize DUP (0)
.code
AsmMedianFilter proc
    ; Parameters:
    ; rcx - bitmap stripe 
    ; rdx - bitmap width
    ; r8 - number of rows in this stripe
    ; r9 - thread number
    ; r10 - y (row)
    ; r11 - x (column)
    ; r12 - current pixel index
    ; rax - current pixel pointer

     ; Calculate unique maskArray address for each thread
    

    ; Now rdi points to the unique maskArray for the current thread

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
    lea rdi, [maskArray]
    mov r13, r9
    shl r13, 3  ; Multiply thread number by 8 (size of QWORD)
    add rdi, r13
    ;(to fix) All threads should have different array adresses ex: maskArray + r9 (thread number) * 9
    ;-------------------------------------top-left
    cmp r11, 0    
    je handle_top_left_edge   
    mov rbx, 0  
    sub rbx, rdx
    sub rbx, 3                  ; rbx = current - width - 3
    add rbx, rax  
    movzx r13, byte ptr [rbx]     
continue_top_left:
    mov [rdi], r13      
    inc rdi
    ;-------------------------------------top-center
    mov rbx, 0                
    sub rbx, rdx                ; rbx = current - width
    add rbx, rax    
    movzx r13, byte ptr [rbx]    
    mov [rdi], r13
    inc rdi

    ;-------------------------------------top-right
    cmp r11, rdx
    je handle_top_right_edge
    mov rbx, 0  
               
    sub rbx, rdx      
    add rbx, 3                  ; rbx = current - width + 3
    add rbx, rax     
    movzx r13, byte ptr [rbx]  
continue_top_right:
    mov [rdi], r13
    inc rdi

    ;-------------------------------------middle-left
    cmp r11, 0
    je handle_middle_left_edge
    mov rbx, 0  
                  
    sub rbx, 3                  ; rbx = current - 3
    add rbx, rax  
    movzx r13, byte ptr [rbx]   
continue_middle_left:
    mov [rdi], r13
    inc rdi
    ;-------------------------------------middle-center
    mov rbx, 0
    add rbx, rax 
    movzx r13, byte ptr [rbx]   
    mov [rdi], r13
    inc rdi
    ;-------------------------------------middle-right
     cmp r11, rdx
    je handle_middle_right_edge
    mov rbx, 0               
    add rbx, 3                  ; rbx = current + 3
    add rbx, rax      
    movzx r13, byte ptr [rbx]  
continue_middle_right:
    mov [rdi], r13
    inc rdi
    ;-------------------------------------bottom-left
    cmp r11, 0
    je handle_bottom_left_edge
    mov rbx, 0                 
    add rbx, rdx
    sub rbx, 3                  ; rbx = current + width - 3
    add rbx, rax 
    movzx r13, byte ptr [rbx]   
continue_bottom_left:
    mov [rdi], r13
    inc rdi
    ;-------------------------------------bottom-center
    mov rbx, 0                
    add rbx, rdx                ; rbx = current + width
    add rbx, rax   
    movzx r13, byte ptr [rbx]    
    mov [rdi], r13
    ;-------------------------------------bottom-right
    cmp r11, rdx
    je handle_bottom_right_edge
    mov rbx, 0  
                 
    add rbx, rdx      
    add rbx, 3                  ; rbx = current + width + 3
    add rbx, rax   
    movzx r13, byte ptr [rbx]   
continue_bottom_right:
    mov [rdi], r13
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
    ; rdi - 3x3 Array
    ; r13 - y
    ; r15 - x
    ; r14 - Number of elements in the array
    ; r11b - current element 
    ; r12b - next element


start_sorting:
    ; Sort the array (simple bubble sort for small arrays)
     
    mov r14b, 9              
    
    mov r13, 0                   ; Outer loop counter
     
outer_loop:
    mov r15b, 0                   ; Inner loop counter
    push r13
    lea rdi, [maskArray]
    mov r13, r9
    shl r13, 3  ; Multiply thread number by 8 (size of QWORD)
    add rdi, r13 
    pop r13

inner_loop:
    mov r11b, [rdi]              ; Load current element
    mov r12b, [rdi + 1]          ; Load next element
    cmp r11b, r12b                 ; Compare current and next element
    jbe no_swap                ; Jump if not greater (no swap needed)

    ; Swap elements
    mov byte ptr [rdi], r12b              ; Store next element at current position
    mov byte ptr [rdi + 1], r11b          ; Store current element at next position

no_swap:
    add r15b, 1                   ; Increment inner loop counter
    inc rdi                     ; Move to the next element
    cmp r15b, r14b              ; Compare with the number of elements
    jl inner_loop               ; Jump if inner loop counter < 9

    add r13, 1                   ; Increment outer loop counter
    cmp r13, 8                  ; Compare with the number of elements - 1
    jl outer_loop               ; Jump if outer loop counter < 8


    ; Select the middle element from the sorted array
    lea rdi, [maskArray]
    mov r13, r9
    shl r13, 3  ; Multiply thread number by 8 (size of QWORD)
    add rdi, r13
    mov r11b, [rdi + 4] 

   ;set current pixel as maskArray pointer
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