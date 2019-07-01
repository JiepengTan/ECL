/*********************************************************** 
 * syntax_indent_demo.c
 **********************************************************/
struct point{int32 x;  int32 y;};
int32 FuncTest(int32 v1,int32 v2){
    int32 a = 0;
    a = a+3;
    return a;
}
void main()
{int32 arr[10]; int32 i; float a2 = 3.56; bool bval = false;	point pt; 
pt.x =1024;pt.y=768;
for(i = 0; i < 10; i = i + 1)	{arr[i]=i;			
if(i == 6){continue;		}
else{printf("arr[%d]=%d\n",i,arr[i]);}}
printf("pt.x = %d, pt.y = %d\n",pt.x,pt.y);}

