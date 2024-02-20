#pragma once

#include "global.h"

NTSTATUS PocInitCommPort();

void PocCloseCommPort();

#define MESSAGE_SIZE				4096*10

#define POC_HELLO_KERNEL			0x00000001
#define POC_ADD_SECURE_FODER		0x00000002
#define POC_ADD_SECURE_EXTENSION	0x00000003
#define POC_PRIVILEGE_DECRYPT		0x00000004
#define POC_GET_PROCESS_RULES		0x00000005
#define POC_GET_FILE_EXTENSION		0x00000006
#define POC_REMOVE_FILE_EXTENSION	0x00000007
#define POC_PRIVILEGE_ENCRYPT		0x00000008
#define POC_ADD_PROCESS_RULES		0x00000009
#define POC_GET_SECURE_FOLDER		0x0000000A
#define POC_REMOVE_SECURE_FOLDER	0x0000000B
#define POC_REMOVE_PROCESS_RULES	0x0000000C

typedef struct _POC_MESSAGE_HEADER
{
	int Command;
	int Length;

} POC_MESSAGE_HEADER, *PPOC_MESSAGE_HEADER;

typedef struct _POC_MESSAGE_PROCESS_RULES
{
	CHAR ProcessName[POC_MAX_NAME_LENGTH];
	ULONG Access;

} POC_MESSAGE_PROCESS_RULES, *PPOC_MESSAGE_PROCESS_RULES;

typedef struct _POC_MESSAGE_SECURE_FODER
{
	CHAR SecureFolder[POC_MAX_NAME_LENGTH];

} POC_MESSAGE_SECURE_FODER, *PPOC_MESSAGE_SECURE_FODER;

typedef struct _POC_MESSAGE_SECURE_EXTENSION
{
	CHAR Extension[POC_EXTENSION_SIZE];

} POC_MESSAGE_SECURE_EXTENSION, *PPOC_MESSAGE_SECURE_EXTENSION;
