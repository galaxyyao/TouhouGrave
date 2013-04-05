#pragma once

#include "ImeUi.h"

namespace TouhouSpring {
namespace Ime {

public enum class ClauseAttribute
{
    Input               = ATTR_INPUT,
    InputError          = ATTR_INPUT_ERROR,
    TargetConverted     = ATTR_TARGET_CONVERTED,
    Converted           = ATTR_CONVERTED,
    TargetNotConverted  = ATTR_TARGET_NOTCONVERTED,
    FixedConverted      = ATTR_FIXEDCONVERTED
};

public value struct CompositionData
{
    bool InComposition;
    System::String^ Text;
    cli::array<ClauseAttribute>^ Attributes;
    int Caret;
};

public value struct CandidateListData
{
    bool IsOpened;
    cli::array<System::String^>^ Candidates;
    int Selection;
    int PageIndex;
    int PageCount;
};

public delegate void KeyMessageHandler(System::Char code);
public delegate void InputLangChangeHandler(System::String^ lang);
public delegate void CompositionMessageHandler(CompositionData data);
public delegate void ParameterlessMessageHandler();
public delegate void CandidateListMessageHandler(CandidateListData data);

public ref class ImeContext
{
public:
    ImeContext(System::IntPtr windowHandle);
    ~ImeContext();

    property bool IsInitialized
    {
        bool get() { return m_initialized; }
    }

    property System::String^ IndicatorString
    {
        System::String^ get();
    }

    void BeginIme();
    void EndIme();

    event ParameterlessMessageHandler^ OnAppActivate;
    event ParameterlessMessageHandler^ OnAppDeactivate;

    event KeyMessageHandler^ OnChar;
    event KeyMessageHandler^ OnKeyDown;
    event KeyMessageHandler^ OnKeyUp;

    event InputLangChangeHandler^ OnInputLangChange;
    event CompositionMessageHandler^ OnCompositionUpdate;
    event CandidateListMessageHandler^ OnCandidateListUpdate;

private:
    delegate LRESULT WndProcDelegate(HWND, UINT, WPARAM, LPARAM);
    LRESULT WindowProcedure(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);
    bool StaticMsgProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);
    void ImeOnInputLangChange();
    void ImeOnCandidateListUpdate();

    static ImeContext^ s_instance;
    bool m_initialized;
    WndProcDelegate^ m_wndProc;
    WNDPROC m_oldWndProc;
    System::Action^ m_imeOnInputLangChange;
    System::Action^ m_imeOnCandidateListUpdate;
};

}
}
