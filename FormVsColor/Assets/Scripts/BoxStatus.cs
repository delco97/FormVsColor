using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoxStatus {
    EMPTY,
    FORM_BLACK_CROSS,
    FORM_WHITE_CIRCLE,
    COLOR_WHITE_CROSS,
    COLOR_BLACK_CIRCLE
}

public static class BoxStatusManager {
    public static List<BoxStatus> GetColorStatuses() {
        return new List<BoxStatus> {
            BoxStatus.EMPTY,
            BoxStatus.COLOR_WHITE_CROSS,
            BoxStatus.COLOR_BLACK_CIRCLE
        };
    }
    
    public static List<BoxStatus> GetFormStatuses() {
        return new List<BoxStatus> {
            BoxStatus.EMPTY,
            BoxStatus.FORM_BLACK_CROSS,
            BoxStatus.FORM_WHITE_CIRCLE
        };
    }    
}
