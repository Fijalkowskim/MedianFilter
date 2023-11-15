.code
AsmMedianFilter proc
   ; Parametry:
    ; rcx - wskaŸnik do danych pikseli
    ; rdx - szerokoœæ obrazu w pikselach
    ; r8 - wysokoœæ obrazu w pikselach
    ; r10 - indeks y
    ; r11 - indeks x
    ; rax - wskaznik na aktualny element rcx

    ; Przyk³adowa pêtla iteruj¹ca po pikselach
    mov r10, 0          ; Indeks wiersza

row_loop:
    cmp r10, r8        ; Sprawdzamy, czy osi¹gnêliœmy koniec obrazu
    jge end_process    ; Jeœli tak, to koñczymy proces

    mov r11, 0          ; Indeks kolumny

column_loop:
    cmp r11, rdx        ; Sprawdzamy, czy osi¹gnêliœmy koniec wiersza
    jge next_row       ; Jeœli tak, przechodzimy do nastêpnego wiersza

    ; Tutaj dodaj kod obs³ugi piksela na pozycji [rdi + (r8 * rdx + r9) * 4]  

    mov rax, r10        ; rax = r8
    imul rax, rdx      ; rax = r8 * rdx
    add rax, r11        ; rax = r8 * rdx + r9
    imul rax, 3        ; rax = (r8 * rdx + r9) * 3
    add rax, rcx       ; rax = rcx + (r8 * rdx + r9) * 3

    ; Modyfikacja piksela
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

    add r11, 1          ; Inkrementujemy indeks kolumny
    jmp column_loop    ; Powtarzamy dla kolejnej kolumny

next_row:
    add r10, 1          ; Inkrementujemy indeks wiersza
    jmp row_loop       ; Powtarzamy dla kolejnego wiersza

end_process:
    ret
AsmMedianFilter endp
end