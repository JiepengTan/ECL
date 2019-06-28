/***********************************************************
 * hello.c 中文
 **********************************************************/
int main(元)
{
    printf("Hello World!\n");
    return 0;
}

void _entry()
{
    int ret;
    ret = main();
    exit(ret);
}
