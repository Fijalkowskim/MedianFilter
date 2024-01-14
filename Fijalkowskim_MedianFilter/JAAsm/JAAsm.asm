; Mateusz Fija³kowski
; Median Filter v1 - 14.01.2024
; Silesian University of Technology 2023/24

; Median filtering - The filtering algorithm involves determining the median (middle value) of the values of neighboring pixels in a mask of a selected size (in my case the mask is 3x3).
; After calculating the median, its value is saved to the currently processed pixel. If a pixel is on an edge, its neighbors are assumed to have a value of 0.
; R, G and B channels are filtered separately.
.data
    swap0 BYTE 1,0,2,3,4,5,6,7,8 ; Arrays for pshufb instruction (swapping bytes in xmm register)
    swap1 BYTE 0,2,1,3,4,5,6,7,8
    swap2 BYTE 0,1,3,2,4,5,6,7,8
    swap3 BYTE 0,1,2,4,3,5,6,7,8
    swap4 BYTE 0,1,2,3,5,4,6,7,8
    swap5 BYTE 0,1,2,3,4,6,5,7,8
    swap6 BYTE 0,1,2,3,4,5,7,6,8
    swap7 BYTE 0,1,2,3,4,5,6,8,7
.code
AsmMedianFilter proc
    ; Parameters:
    ; rcx       - bitmap pointer 
    ; rdx       - bitmap width
    ; r8        - rows to filter
    ; r9        - start row
    ; [rsp+40]  - bitmap height

    ; Used registers:
    ; r10       - y (row)
    ; r11       - x (column)
    ; r12       - current pixel index
    ; rax       - current pixel pointer

    ; Used sse instructions:
    ; pinsrb (ex. pinsrb xmm0, r13, 0 - Insert byte from r13 into xmm0 at index 0) 
    ; pextrb (ex. pextrb xmm0, r13, 3 - Exctract byte from xmm0 at index 3 into r13)
    ; movaps (ex. movaps xmm0, xmm1 - Move values from xmm1 to xmm0)
    ; movdqu (ex. movdqu  xmm3, xmmword ptr [swap5] - Move swap5 byte array casted to 128-bit xmmword into xmm3 register )
    ; pshufb (ex. pshufb  xmm0, xmm3 - Shuffle bytes inside xmm0 based on bytes in xmm3 register
    ;                                  if xmm3 bytes are: 1 0 2 3 4 5 6 7 8 9 10 11 12 13 14 15 then xmm0 bytes 0 and 1 are swapped)

    mov r10, r9         ; Initialize row = startRow           
    mov r12, r9         ; Initialize start pixel = startRow * bitmapWidth    
    imul r12, rdx

row_loop:
    mov r13, r8
    add r13, r9         ; r13 = rows + start row
    cmp r10, r13        ; Check if we have reached the end of the stripe
    jge end_process     ; If so, end the process
    mov r11, 0          ; Initialize column index

column_loop:
    cmp r11, rdx        ; Check if we have reached the end of the row
    jge next_row        ; If so, move to the next row
                      
    mov rax, r12        ; Set current pixel pointer (bitmap pointer + current pixel index)
    add rax, rcx

    ;---------------Calculate 3x3 mask---------------
    ; Used registers:
    ; rbx - current neighbour pixel pointer
    ; r13b, r14b, r15b - R,G,B values of that pixel
    ; xmm0 - 3x3 mask with red values
    ; xmm1 - 3x3 mask with green values
    ; xmm2 - 3x3 mask with blue values
    ;-------------------------------------top-left

    cmp r11, 0              ; Check if y == 0 or x == 0 (edge)
    je handle_top_left      ; If so, handle egde
    cmp r10, 0
    je handle_top_left  
    mov rbx, rax            ; rbx = current pixel pointer - width - 3
    sub rbx, rdx
    sub rbx, 3                 
    call mask_to_registers  ; Go to procedure moving RGB values to r13b,r14b,r15b
    jmp continue_top_left   ; Jump over edge handling
handle_top_left:
   call handle_edge         ; Go to handle edge procedure
continue_top_left:
    pinsrb xmm0, r13, 0     ; Move red   value to byte 0 of xmm0
    pinsrb xmm1, r14, 0     ; Move green value to byte 0 of xmm1
    pinsrb xmm2, r15, 0     ; Move blue  value to byte 0 of xmm2
    ;-------------------------------------top-center

    cmp r10, 0              ; Check if y == 0
    je handle_top_center    ; If so, handle egde
    mov rbx, rax    
    sub rbx, rdx            ; rbx = current pixel pointer - width      
    call mask_to_registers  ; Go to procedure moving RGB values to r13b,r14b,r15b 
    jmp continue_top_center ; Jump over edge handling
handle_top_center:
    call handle_edge
continue_top_center:
    pinsrb xmm0, r13, 1     ; Move red   value to byte 1 of xmm0
    pinsrb xmm1, r14, 1     ; Move green value to byte 1 of xmm1
    pinsrb xmm2, r15, 1     ; Move blue  value to byte 1 of xmm2
    ;-------------------------------------top-right

    cmp r10, 0              ; Check if y == 0 or x == bitmatWidth - 3 (edge)  
    je handle_top_right     ; If so, handle egde
    mov r13, rdx
    sub r13, 3
    cmp r11, r13
    je handle_top_right
    mov rbx, rax              
    sub rbx, rdx      
    add rbx, 3              ; rbx = current pixel pointer - width + 3     
    call mask_to_registers  ; Go to procedure moving RGB values to r13b,r14b,r15b
    jmp continue_top_right  ; Jump over edge handling
handle_top_right:
   call handle_edge
continue_top_right:
    pinsrb xmm0, r13, 2     ; Move red   value to byte 2 of xmm0 
    pinsrb xmm1, r14, 2     ; Move green value to byte 2 of xmm1 
    pinsrb xmm2, r15, 2     ; Move blue  value to byte 2 of xmm2 
    ;-------------------------------------middle-left

    cmp r11, 0              ; Check if x == 0 (edge)
    je handle_middle_left   ; If so, handle egde
    mov rbx, rax                
    sub rbx, 3              ; rbx = current pixel pointer - 3   
    call mask_to_registers  ; Go to procedure moving RGB values to r13b,r14b,r15b
    jmp continue_middle_left; Jump over edge handling
handle_middle_left:
   call handle_edge
continue_middle_left:
    pinsrb xmm0, r13, 3     ; Move red   value to byte 3 of xmm0      
    pinsrb xmm1, r14, 3     ; Move green value to byte 3 of xmm1 
    pinsrb xmm2, r15, 3     ; Move blue  value to byte 3 of xmm2 
    ;-------------------------------------middle-center

    mov rbx, rax            ; rbx = current pixel pointer
    call mask_to_registers  ; Go to procedure moving RGB values to r13b,r14b,r15b
    pinsrb xmm0, r13, 4     ; Move red   value to byte 4 of xmm0        
    pinsrb xmm1, r14, 4     ; Move green value to byte 4 of xmm1 
    pinsrb xmm2, r15, 4     ; Move blue  value to byte 4 of xmm2 
    ;-------------------------------------middle-right

    mov r13, rdx            ; Check if x == bitmapWidth - 3 (edge)
    sub r13, 3              ; If so, handle egde
    cmp r11, r13
je handle_middle_right
    mov rbx, rax               
    add rbx, 3              ; rbx = current pixel pointer + 3      
    call mask_to_registers   ; Go to procedure moving RGB values to r13b,r14b,r15b
    jmp continue_middle_right; Jump over edge handling
handle_middle_right:
   call handle_edge
continue_middle_right:
    pinsrb xmm0, r13, 5     ; Move red   value to byte 5 of xmm0 
    pinsrb xmm1, r14, 5     ; Move green value to byte 5 of xmm1 
    pinsrb xmm2, r15, 5     ; Move blue  value to byte 5 of xmm2 
    ;-------------------------------------bottom-left

    cmp r11, 0              ; Check if x == 0 or y == bitmapHeight - 1 (edge)
    je handle_bottom_left   ; If so, handle egde
    mov r13, [rsp+40]
    sub r13, 1
    cmp r10, r13
    je handle_bottom_left
    mov rbx, rax                 
    add rbx, rdx
    sub rbx, 3              ; rbx = current pixel pointer + width - 3 
    call mask_to_registers  ; Go to procedure moving RGB values to r13b,r14b,r15b
    jmp continue_bottom_left; Jump over edge handling
handle_bottom_left:
   call handle_edge
continue_bottom_left:
    pinsrb xmm0, r13, 6     ; Move red   value to byte 6 of xmm0
    pinsrb xmm1, r14, 6     ; Move green value to byte 6 of xmm1
    pinsrb xmm2, r15, 6     ; Move blue  value to byte 6 of xmm2
    ;-------------------------------------bottom-center

    mov r13, [rsp+40]       ; Check if y == bitmapHeight - 1 (edge)
    sub r13, 1              ; If so, handle egde
    cmp r10, r13
    je handle_bottom_center
    mov rbx, rax                
    add rbx, rdx            ; rbx = current pixel pointer + width   
    call mask_to_registers    ; Go to procedure moving RGB values to r13b,r14b,r15b
    jmp continue_bottom_center; Jump over edge handling
handle_bottom_center:
   call handle_edge
continue_bottom_center:
    pinsrb xmm0, r13, 7     ; Move red   value to byte 7 of xmm0
    pinsrb xmm1, r14, 7     ; Move green value to byte 7 of xmm1
    pinsrb xmm2, r15, 7     ; Move blue  value to byte 7 of xmm2
    ;-------------------------------------bottom-right

    mov r13, [rsp+40]       ; Check if y == bitmapHeight - 1 or x == bitmapWidth - 3 (edge)
    sub r13, 1              ; If so, handle egde
    cmp r10, r13
    je handle_bottom_right
    mov r13, rdx
    sub r13, 3
    cmp r11, r13
    je handle_bottom_right
    mov rbx, rax                  
    add rbx, rdx      
    add rbx, 3              ; rbx = current pixel pointer + width + 3   
    call mask_to_registers   ; Go to procedure moving RGB values to r13b,r14b,r15b
    jmp continue_bottom_right; Jump over edge handling
handle_bottom_right:
   call handle_edge
continue_bottom_right:
    pinsrb xmm0, r13, 8     ; Move red   value to byte 8 of xmm0
    pinsrb xmm1, r14, 8     ; Move green value to byte 8 of xmm1
    pinsrb xmm2, r15, 8     ; Move blue  value to byte 8 of xmm2

    call start_sorting      ; Start sorting procedure for red channel
    movaps xmm0, xmm1       ; Move green values from xmm1 to xmm0
    inc rax                 ; Move to next pixel color channel (green)
    call start_sorting      ; Start sorting procedure for green channel
    movaps xmm0, xmm2       ; Move blue values from xmm2 to xmm0
    inc rax                 ; Move to next pixel color channel (blue)            
    call start_sorting      ; Start sorting procedure for blue channel
    jmp next_pixel          ; Jump to next_pixel
;---------------/Calculate 3x3 mask---------------

;-------------------------------------------------SORTING-------------------------------------------------
    ; Simple bubble sort for small arrays
    ; Used registers:
    ; xmm0 - 3x3 masks with neighbour R,G or B values
    ; xmm3 - register with array of indexes for swapping bytes inside xmm0
    ; rbx - loop counter (repeat 8 times)
    ; r13 - current pixel value in mask
    ; r14 - next pixel value in mask
start_sorting:                 
    mov rbx, 0                          ; loop counter
inner_loop:
    pextrb r13, xmm0, 0                 ; Load current element  (byte 0 from xmm0)
    pextrb r14, xmm0, 1                 ; Load next element     (byte 1 from xmm0)
    cmp r13, r14                        ; Compare current and next element
    jbe no_swap_1                       ; Jump if not greater (no swap needed)
    movdqu  xmm3, xmmword ptr [swap0]   ; Move swap0 array into xmm3 (swapped bytes 0 and 1)
    pshufb  xmm0, xmm3                  ; Swap xmm0 bytes based on xmm3 byte values
no_swap_1:
    pextrb r13, xmm0, 1                 ; Load current element  (byte 1 from xmm0)   
    pextrb r14, xmm0, 2                 ; Load next element     (byte 2 from xmm0)   
    cmp r13, r14                        ; Compare current and next element   
    jbe no_swap_2                       ; Jump if not greater (no swap needed)                    
    movdqu  xmm3, xmmword ptr [swap1]   ; Move swap1 array into xmm3 (swapped bytes 1 and 2)
    pshufb  xmm0, xmm3                  ; Swap xmm0 bytes based on xmm3 byte values
no_swap_2:
    pextrb r13, xmm0, 2                 ; Load current element  (byte 2 from xmm0)      
    pextrb r14, xmm0, 3                 ; Load next element     (byte 3 from xmm0)
    cmp r13, r14                        ; Compare current and next element
    jbe no_swap_3                       ; Jump if not greater (no swap needed)
    movdqu  xmm3, xmmword ptr [swap2]   ; Move swap2 array into xmm3 (swapped bytes 2 and 3)
    pshufb  xmm0, xmm3                  ; Swap xmm0 bytes based on xmm3 byte values
no_swap_3:
    pextrb r13, xmm0, 3                 ; Load current element  (byte 3 from xmm0)   
    pextrb r14, xmm0, 4                 ; Load next element     (byte 4 from xmm0)   
    cmp r13, r14                        ; Compare current and next element   
    jbe no_swap_4                       ; Jump if not greater (no swap needed)              
    movdqu  xmm3, xmmword ptr [swap3]   ; Move swap3 array into xmm3 (swapped bytes 3 and 4)
    pshufb  xmm0, xmm3                  ; Swap xmm0 bytes based on xmm3 byte values
no_swap_4:
    pextrb r13, xmm0, 4                 ; Load current element  (byte 4 from xmm0)
    pextrb r14, xmm0, 5                 ; Load next element     (byte 5 from xmm0)
    cmp r13, r14                        ; Compare current and next element
    jbe no_swap_5                       ; Jump if not greater (no swap needed)
    movdqu  xmm3, xmmword ptr [swap4]   ; Move swap4 array into xmm3 (swapped bytes 4 and 5)
    pshufb  xmm0, xmm3                  ; Swap xmm0 bytes based on xmm3 byte values
no_swap_5:
    pextrb r13, xmm0, 5                 ; Load current element  (byte 5 from xmm0)   
    pextrb r14, xmm0, 6                 ; Load next element     (byte 6 from xmm0)   
    cmp r13, r14                        ; Compare current and next element   
    jbe no_swap_6                       ; Jump if not greater (no swap needed)                 
    movdqu  xmm3, xmmword ptr [swap5]   ; Move swap5 array into xmm3 (swapped bytes 5 and 6)
    pshufb  xmm0, xmm3                  ; Swap xmm0 bytes based on xmm3 byte values
no_swap_6:
    pextrb r13, xmm0, 6                 ; Load current element  (byte 6 from xmm0)      
    pextrb r14, xmm0, 7                 ; Load next element     (byte 7 from xmm0)
    cmp r13, r14                        ; Compare current and next element
    jbe no_swap_7                       ; Jump if not greater (no swap needed)
    movdqu  xmm3, xmmword ptr [swap6]   ; Move swap6 array into xmm3 (swapped bytes 6 and 7)
    pshufb  xmm0, xmm3                  ; Swap xmm0 bytes based on xmm3 byte values
no_swap_7:
    pextrb r13, xmm0, 7                 ; Load current element  (byte 7 from xmm0)   
    pextrb r14, xmm0, 8                 ; Load next element     (byte 8 from xmm0)   
    cmp r13, r14                        ; Compare current and next element   
    jbe no_swap_8                       ; Jump if not greater (no swap needed)              
    movdqu  xmm3, xmmword ptr [swap7]   ; Move swap7 array into xmm3 (swapped bytes 7 and 8)
    pshufb  xmm0, xmm3                  ; Swap xmm0 bytes based on xmm3 byte values
no_swap_8:
    add rbx, 1                          ; Increment inner loop counter
    cmp rbx, 8                          ; Compare with the number of elements - 1
    jl inner_loop                       ; Jump if inner loop counter < 8   
                                        ; xmm0 is sorted
    pextrb r13, xmm0, 4                 ; Move the median (4th) element from the sorted array 
    mov byte ptr [rax], r13b            ; Set current bitmap pixel as median value 
    ret   
;-------------------------------------------------/SORTING-------------------------------------------------
next_pixel:
    add r11, 3      ; Increment column index by 3 (R,G,B)
    add r12, 3      ; Increment pixel index by 3 (R,G,B)
    jmp column_loop ; Repeat for the next column
next_row:
    add r10, 1      ; Increment row index
    jmp row_loop    ; Repeat for the next row
mask_to_registers:              ; Procedure for moving color channel values intro registers
    mov r13b, byte ptr [rbx]    ; Red to r13b
    inc rbx                     ; Next color channel
    mov r14b, byte ptr [rbx]    ; Green to r14b
    inc rbx                     ; Next color channel
    mov r15b, byte ptr [rbx]    ; Blue to r15b
    ret
handle_edge:        ; Edge handling for calculating 3x3 neighbours mask
    mov r13b, 0     ; Set zeros as RGB values
    mov r14b, 0
    mov r15b, 0
    ret
end_process:  
    ret
AsmMedianFilter endp
end