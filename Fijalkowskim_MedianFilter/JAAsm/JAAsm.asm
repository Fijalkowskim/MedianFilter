.code
AsmMedianFilter proc
   ; Parametry:
    ; rcx - wska�nik do danych pikseli
    ; rdx - szeroko�� obrazu w pikselach
    ; r8 - wysoko�� obrazu w pikselach
    ; r10 - indeks y
    ; r11 - indeks x
    ; rax - wskaznik na aktualny element rcx

    ; Przyk�adowa p�tla iteruj�ca po pikselach
    mov r10, 0          ; Indeks wiersza

row_loop:
    cmp r10, r8        ; Sprawdzamy, czy osi�gn�li�my koniec obrazu
    jge end_process    ; Je�li tak, to ko�czymy proces

    mov r11, 0          ; Indeks kolumny

column_loop:
    cmp r11, rdx        ; Sprawdzamy, czy osi�gn�li�my koniec wiersza
    jge next_row       ; Je�li tak, przechodzimy do nast�pnego wiersza

    ; Tutaj dodaj kod obs�ugi piksela na pozycji [rdi + (r8 * rdx + r9) * 4]  

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