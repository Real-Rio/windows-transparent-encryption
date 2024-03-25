#include "pch.h"
#include <iostream>

#define CSP_NAME  "Longmai mToken GM3000 CSP V1.1" // UKey����
#define ContName  "Cer" // UKey��������


#define AUTHORIZATION_NOUSBKEY 0
#define AUTHORIZATION_NOSIGNATUREKEY 1
#define AUTHORIZATION_ERRGETCERT 2
#define AUTHORIZATION_ERRMALLOC 3
#define AUTHORIZATION_ERRGETCERTCONTEXT 4
#define	AUTHORIZATION_ERRGETCERTSTORE 5
#define AUTHORIZATION_SUCCESS 6
#define AUTHORIZATION_FALSE 7

#pragma comment(lib, "crypt32.lib")
INT AuthorizeAndOpenDriver() {
	HCRYPTPROV hCryptProv = NULL;
	HCRYPTKEY hKey = NULL;
	DWORD certLen=0;
	BYTE* pbCert = NULL;
	PCCERT_CONTEXT pUserCertContext = NULL, pCaCertContext = NULL;
	HCERTSTORE hCertStore = NULL;
	DWORD verifyFlag = CERT_STORE_SIGNATURE_FLAG | CERT_STORE_TIME_VALIDITY_FLAG; //��֤֤���Ƿ��ɸ�֤��ǩ�����Ƿ����
	int retValue = -1;


	if (!CryptAcquireContextA(
		&hCryptProv,
		ContName,
		CSP_NAME,
		PROV_RSA_FULL,
		NULL
	))
	{
		retValue = AUTHORIZATION_NOUSBKEY;
		goto cleanup;
	}

	// ��ȡ��Կ���
	if (!CryptGetUserKey(
		hCryptProv,
		AT_SIGNATURE,
		&hKey))
	{
		retValue = AUTHORIZATION_NOSIGNATUREKEY;
		goto cleanup;
	}

	// ����˽Կ��ȡ֤������
	// �Ȼ�ȡ֤�����ݳ���
	if (!CryptGetKeyParam(
		hKey,
		KP_CERTIFICATE,
		NULL,
		&certLen,
		0)) 
	{
		retValue = AUTHORIZATION_ERRGETCERT;
		goto cleanup;
	}

	pbCert = (BYTE*)new BYTE[certLen];
	if (pbCert == NULL)
	{
		retValue = AUTHORIZATION_ERRMALLOC;
		goto cleanup;
	}

	if (!CryptGetKeyParam(
		hKey,
		KP_CERTIFICATE,
		pbCert,
		&certLen,
		0))
	{
		retValue = AUTHORIZATION_ERRGETCERT;
		goto cleanup;
	}

	// ��֤������ת��ΪcertContext
	pUserCertContext = CertCreateCertificateContext(
		X509_ASN_ENCODING | PKCS_7_ASN_ENCODING,
		pbCert,
		certLen
	);

	if (pUserCertContext == NULL)
	{
		retValue = AUTHORIZATION_ERRGETCERTCONTEXT;
		goto cleanup;
	}	

	// ��ȡCA certStore
	hCertStore = CertOpenStore(
		CERT_STORE_PROV_SYSTEM,   // The store provider type.
		0,                        // The encoding type is not needed.
		NULL,               // Use the epassNG HCRYPTPROV.
		CERT_SYSTEM_STORE_CURRENT_USER,
		L"Root"
	);

	if (hCertStore == NULL)
	{
		retValue = AUTHORIZATION_ERRGETCERTSTORE;
		goto cleanup;
	}

	// ��֤֤��
	pCaCertContext = CertGetIssuerCertificateFromStore(
		hCertStore,
		pUserCertContext,
		NULL,
		&verifyFlag
	);

	if (pCaCertContext == NULL) {
		retValue = AUTHORIZATION_ERRGETCERTCONTEXT;
		goto cleanup;
	}
	else {
		if (verifyFlag != 0) {
			//cout << "The issuer certificate is not valid. Return flag is " << flag << endl;
			retValue = AUTHORIZATION_FALSE;
			goto cleanup;
		}
		else {
			//cout << "The issuer certificate is valid." << endl;
			//printCertName(pCaCertContext);
			retValue = AUTHORIZATION_SUCCESS;
			goto cleanup;
		}
	}

cleanup:

	if (pUserCertContext != NULL)
	{
		CertFreeCertificateContext(pUserCertContext);
	}

	if (pCaCertContext != NULL)
	{
		CertFreeCertificateContext(pCaCertContext);
	}

	if (hCertStore != NULL)
	{
		CertCloseStore(hCertStore, 0);
	}

	if (pbCert != NULL)
	{
		delete[] pbCert;
	}

	if (hKey != NULL)
	{
		CryptDestroyKey(hKey);
	}

	if (hCryptProv != NULL)
	{
		CryptReleaseContext(hCryptProv, 0);
	}

	return retValue;
}