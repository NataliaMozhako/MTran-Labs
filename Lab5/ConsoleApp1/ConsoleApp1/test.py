def print_result(A_: np.array):
    for j in range(len(A_)):
        print(" ".join((map(str, A_[j]))))

n, i = tuple(map(int, input().split()))

matrix_A = np.zeros((n,n))
matrix_B = np.zeros((n,n))

for j in range(n):
    matrix_A[j] = list(map(float, input().split()))
for j in range(n):
    matrix_B[j] = list(map(float, input().split()))

vect_x = list(map(float, input().split()))
matrix_A[ :, i - 1] = vect_x

vect1_l = matrix_B @ vect_x