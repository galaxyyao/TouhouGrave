#include "ImeContext.h"
#include <malloc.h>

using namespace System::Collections::Generic;

namespace TouhouSpring {
namespace Ime {

ImeContext::ImeContext(System::IntPtr windowHandle)
{
    if (s_instance != nullptr)
    {
        throw gcnew System::InvalidOperationException("Only one instance of ImeContext can be created.");
    }
    s_instance = this;

    HWND hWnd = reinterpret_cast<HWND>(windowHandle.ToPointer());

    m_wndProc = gcnew WndProcDelegate(this, &ImeContext::WindowProcedure);
    System::IntPtr funcPtr = System::Runtime::InteropServices::Marshal::GetFunctionPointerForDelegate(m_wndProc);
    m_oldWndProc = reinterpret_cast<WNDPROC>(::SetWindowLongPtr(hWnd, GWLP_WNDPROC, reinterpret_cast<LONG>(funcPtr.ToPointer())));

    m_imeOnInputLangChange = gcnew System::Action(this, &ImeContext::ImeOnInputLangChange);
    funcPtr = System::Runtime::InteropServices::Marshal::GetFunctionPointerForDelegate(m_imeOnInputLangChange);
    typedef void (CALLBACK *ImeOnInputLangChangeCallback)();
    ImeUiCallback_OnInputLangChange = reinterpret_cast<ImeOnInputLangChangeCallback>(funcPtr.ToPointer());

    //m_imeOnCandidateListUpdate = gcnew System::Action(this, &ImeContext::ImeOnCandidateListUpdate);
    //funcPtr = System::Runtime::InteropServices::Marshal::GetFunctionPointerForDelegate(m_imeOnCandidateListUpdate);
    //typedef void (CALLBACK *ImeOnCandidateListUpdate)();
    //ImeUiCallback_OnCandidateListUpdate = reinterpret_cast<ImeOnCandidateListUpdate>(funcPtr.ToPointer());

    ImeUiCallback_DrawRect = NULL;
    ImeUiCallback_Malloc = malloc;
    ImeUiCallback_Free = free;
    ImeUiCallback_DrawFans = NULL;

    m_initialized = ImeUi_Initialize(hWnd, true, false);
    if (!m_initialized)
    {
        return;
    }

    ImeUi_EnableIme( true );
}

ImeContext::~ImeContext()
{
    ImeUi_Uninitialize();
}

System::String^ ImeContext::IndicatorString::get()
{
    return gcnew System::String(ImeUi_IsEnabled() ? ImeUi_GetIndicatior() : L"En");
}

void ImeContext::BeginIme()
{
    ImeUi_EnableIme(true);
}

void ImeContext::EndIme()
{
    ImeUi_FinalizeString();
    ImeUi_EnableIme(false);
}

LRESULT ImeContext::WindowProcedure(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
    bool overrideDefault = StaticMsgProc(hWnd, msg, wParam, lParam);
    if (overrideDefault)
    {
        return 0;
    }

    ImeUi_ProcessMessage( hWnd, msg, wParam, lParam, &overrideDefault );

    if (m_initialized)
    {
        switch (msg)
        {
        case WM_IME_COMPOSITION:
            {
                CompositionData data;
                data.InComposition = true;
                data.Text = gcnew System::String(ImeUi_GetCompositionString());
                data.Attributes = gcnew cli::array<ClauseAttribute>(data.Text->Length);
                BYTE* attr = ImeUi_GetCompStringAttr();
                assert(attr != NULL);
                for (int i = 0; i < data.Attributes->Length; ++i)
                {
                    data.Attributes[i] = safe_cast<ClauseAttribute>(attr[i]);
                }
                data.Caret = ImeUi_GetImeCursorChars();
                OnCompositionUpdate(data);
            }
            break;
        case WM_IME_ENDCOMPOSITION:
            {
                CompositionData data;
                data.InComposition = false;
                OnCompositionUpdate(data);
            }
            break;
		case WM_IME_NOTIFY:
			switch (wParam)
			{
			case IMN_OPENCANDIDATE:
			case IMN_CHANGECANDIDATE:
				UpdateCandidateListFromImm(hWnd);
				break;
			case IMN_CLOSECANDIDATE:
				CandidateListData data;
				data.IsOpened = false;
				OnCandidateListUpdate(data);
				break;
			default:
				break;
			}
			break;
        default:
            break;
        }
    }
    if (overrideDefault)
    {
        return 0;
    }

    if (msg == WM_CHAR)
    {
        OnChar(static_cast<WCHAR>(wParam));
    }
    else if (msg == WM_KEYDOWN)
    {
        OnKeyDown(static_cast<WCHAR>(wParam));
    }
    else if (msg == WM_KEYUP)
    {
        OnKeyUp(static_cast<WCHAR>(wParam));
    }
    else if (msg == WM_ACTIVATEAPP)
    {
        if (wParam != 0)
        {
            ::SetFocus(hWnd); // IME only activates if the window gets the focus...
            OnAppActivate();
        }
        else
        {
            OnAppDeactivate();
        }
    }

    // Default message processing
    LRESULT retCode = ::CallWindowProc(m_oldWndProc, hWnd, msg, wParam, lParam);

    if (msg == WM_GETDLGCODE)
    {
        retCode |= DLGC_WANTALLKEYS | DLGC_WANTCHARS;
    }

    return retCode;
}

bool ImeContext::StaticMsgProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
    if( !ImeUi_IsEnabled() )
        return false;

#if defined(DEBUG) || defined(_DEBUG)
    //m_bIMEStaticMsgProcCalled = true;
#endif

    switch( uMsg )
    {
    case WM_INPUTLANGCHANGE:
        //System::Diagnostics::Debug::WriteLine("WM_INPUTLANGCHANGE");
        return true;

    case WM_IME_SETCONTEXT:
        //System::Diagnostics::Debug::WriteLine(System::String::Format("WM_IME_SETCONTEXT {0} {1}", wParam, lParam));
        //
        // We don't want anything to display, so we have to clear this
        //
        lParam = 0;
        return false;

        // Handle WM_IME_STARTCOMPOSITION here since
        // we do not want the default IME handler to see
        // this when our fullscreen app is running.
    case WM_IME_STARTCOMPOSITION:
        //System::Diagnostics::Debug::WriteLine("WM_IME_STARTCOMPOSITION");
        //ResetCompositionString();
        // Since the composition string has its own caret, we don't render
        // the edit control's own caret to avoid double carets on screen.
        //s_bHideCaret = true;
        return true;
    case WM_IME_ENDCOMPOSITION:
        //System::Diagnostics::Debug::WriteLine("WM_IME_ENDCOMPOSITION");
        //s_bHideCaret = false;
        return false;
    case WM_IME_COMPOSITION:
        //System::Diagnostics::Debug::WriteLine("WM_IME_COMPOSITION");
        return false;
    }

    return false;
}

void ImeContext::ImeOnInputLangChange()
{
    OnInputLangChange(IndicatorString);
}

void ImeContext::ImeOnCandidateListUpdate()
{
    CandidateListData data;
    data.IsOpened = ImeUi_IsShowCandListWindow() && ImeUi_GetCandidateCount() > 0;
    if (data.IsOpened)
    {
        List<System::String^>^ candidates = gcnew List<System::String^>(ImeUi_GetCandidateCount());
        for (UINT i = 0; i < ImeUi_GetCandidateCount(); ++i)
        {
            TCHAR* str = ImeUi_GetCandidate(i);
            if (*str == L'\0')
            {
                break;
            }
            candidates->Add(gcnew System::String(str));
        }
        data.Candidates = candidates->ToArray();
        data.Selection = safe_cast<int>(ImeUi_GetCandidateSelection());
		data.HasPreviousPage = ImeUi_GetCandidatePageIndex() > 0;
		data.HasNextPage = ImeUi_GetCandidatePageIndex() < ImeUi_GetCandidatePageCount() - 1;
    }
    OnCandidateListUpdate(data);
}

void ImeContext::UpdateCandidateListFromImm(HWND hWnd)
{
    HIMC himc = ::ImmGetContext(hWnd);
    if (himc == NULL)
    {
        return;
    }

    CandidateListData data;

    DWORD buffSize = ::ImmGetCandidateList(himc, 0, NULL, 0);
    if (buffSize > 0)
    {
        ::CANDIDATELIST* pCandList = reinterpret_cast<::CANDIDATELIST*>(_alloca(buffSize));
        ::ImmGetCandidateList(himc, 0, pCandList, buffSize);

        if (pCandList->dwCount > 0)
        {
            data.IsOpened = true;
			data.Selection = safe_cast<int>(pCandList->dwSelection);
			data.Selection -= pCandList->dwPageStart;
			data.HasPreviousPage = pCandList->dwPageStart > 0;
			data.HasNextPage = pCandList->dwPageStart + pCandList->dwPageSize < pCandList->dwCount;

            List<System::String^>^ candidates = gcnew List<System::String^>(pCandList->dwPageSize);
            for (UINT i = 0; i < pCandList->dwPageSize; ++i)
            {
				DWORD strOffset = pCandList->dwOffset[i + pCandList->dwPageStart];
                const TCHAR* str = reinterpret_cast<const TCHAR*>(reinterpret_cast<char*>(pCandList) + strOffset);
                candidates->Add(gcnew System::String(str));
            }
            data.Candidates = candidates->ToArray();

			//System::Diagnostics::Trace::WriteLine(System::String::Format("Count:{2} PageStart:{0} PageSize:{1}", pCandList->dwPageStart, pCandList->dwPageSize, pCandList->dwCount));
        }

        _freea(pCandList);
    }

    ::ImmReleaseContext(hWnd, himc);

	OnCandidateListUpdate(data);
}

}
}
