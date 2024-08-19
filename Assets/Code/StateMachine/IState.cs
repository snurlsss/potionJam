using System;
using System.Collections.Generic;

public interface IState
{
    void Tick();
    void OnEnter();
    void OnExit();
}
