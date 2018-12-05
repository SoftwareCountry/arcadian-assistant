import React, { Component } from 'react';
import { calendarStyles } from './styles';
import { Animated, Easing, LayoutChangeEvent, StyleSheet, TranslateXTransform, View } from 'react-native';
import { CalendarPage, OnSelectedDayCallback } from './calendar-page';
import {
    CalendarPageModel,
    CalendarSelection,
    DayModel,
    ReadOnlyIntervalsModel
} from '../reducers/calendar/calendar.model';
import {
    PanGestureHandler,
    PanGestureHandlerEventExtra,
    PanGestureHandlerStateChangeEvent,
    State
} from 'react-native-gesture-handler';

//============================================================================
interface CalendarPagerDefaultProps {
    intervals?: ReadOnlyIntervalsModel;
    selection?: CalendarSelection;
    disableBefore?: DayModel;
}

//============================================================================
interface CalendarPagerProps extends CalendarPagerDefaultProps {
    pages: CalendarPageModel[];
    onSelectedDay: OnSelectedDayCallback;
    onNextPage: () => void;
    onPrevPage: () => void;
}

//============================================================================
interface CalendarPagerState {
    width: number;
    height: number;
}

//============================================================================
export class CalendarPager extends Component<CalendarPagerProps, CalendarPagerState> {
    public static defaultProps: CalendarPagerDefaultProps = {
        intervals: undefined,
        disableBefore: undefined
    };

    private readonly motionThreshold = 120;
    private coordinates = new Animated.ValueXY({ x: 0, y: 0 });
    private canSwipe = true;

    //----------------------------------------------------------------------------
    private onPanGestureEvent = Animated.event(
        [{ nativeEvent: { translationX: this.coordinates.x, translationY: this.coordinates.y }}],
        { useNativeDriver: true }
    );

    //----------------------------------------------------------------------------
    constructor(props: CalendarPagerProps) {
        super(props);
        this.state = {
            width: 0,
            height: 0,
        };
    }

    //----------------------------------------------------------------------------
    public render() {
        const [ translateXProperty ] = this.coordinates.getTranslateTransform() as [{ 'translateX': any }];

        const animatedViewStyle = StyleSheet.flatten([
            calendarStyles.swipeableList,
            { transform: [translateXProperty as TranslateXTransform] },
            { left: -this.state.width }
        ]);

        return (
            <View
                style={calendarStyles.pagerContainer}
                onLayout={this.onLayoutContainer}>
                <PanGestureHandler
                    minDist={20}
                    onGestureEvent={this.onPanGestureEvent}
                    onHandlerStateChange={this.onPanHandlerStateChange}>
                    <Animated.View
                        style={animatedViewStyle}>
                        {
                            this.props.pages.map(page =>
                                <CalendarPage
                                    key={page.pageId}
                                    selection={this.props.selection}
                                    onSelectedDay={this.props.onSelectedDay}
                                    pageDate={page.date}
                                    weeks={page.weeks}
                                    intervals={this.props.intervals}
                                    disableBefore={this.props.disableBefore}
                                    width={this.state.width}
                                    height={this.state.height}
                                />)
                        }
                    </Animated.View>
                </PanGestureHandler>
            </View>
        );
    }

    //----------------------------------------------------------------------------
    private onPanHandlerStateChange = (event: PanGestureHandlerStateChangeEvent) => {
        const gesture = event.nativeEvent;

        switch (gesture.state) {
            case State.BEGAN:
                if (!this.canSwipe) {
                    return;
                }
                break;

            case State.END:
                if (this.rightToLeftSwipe(gesture)) {
                    this.canSwipe = false;
                    this.moveToPage(gesture, -this.state.width, () => this.nextPage(), this.currentPage.isPageLast);
                } else if (this.leftToRightSwipe(gesture)) {
                    this.canSwipe = false;
                    this.moveToPage(gesture, this.state.width, () => this.prevPage(), this.currentPage.isPageFirst);
                }
                return;

            default:
                break;
        }
    };

    //----------------------------------------------------------------------------
    private rightToLeftSwipe(gesture: PanGestureHandlerEventExtra): boolean {
        return gesture.translationX < 0;
    }

    //----------------------------------------------------------------------------
    private leftToRightSwipe(gesture: PanGestureHandlerEventExtra): boolean {
        return gesture.translationX > 0;
    }

    //----------------------------------------------------------------------------
    private nextPage() {
        this.props.onNextPage();
    }

    //----------------------------------------------------------------------------
    private prevPage() {
        this.props.onPrevPage();
    }

    //----------------------------------------------------------------------------
    private get currentPage(): CalendarPageModel {
        return this.props.pages[1];
    }

    //----------------------------------------------------------------------------
    private moveToPage(gesture: PanGestureHandlerEventExtra, toValue: number, onCompleteMove: () => void, isFirstOrLast: boolean) {
        if (this.isThresholdExceeded(gesture) && !isFirstOrLast) {
            this.moveToNearestPage(toValue, () => {
                onCompleteMove();
            });
        } else {
            this.moveToNearestPage(0);
        }
    }

    //----------------------------------------------------------------------------
    private isThresholdExceeded(gesture: PanGestureHandlerEventExtra) {
        return this.motionThreshold - Math.abs(gesture.translationX) <= 0;
    }

    //----------------------------------------------------------------------------
    private moveToNearestPage(toValue: number, onMoveComplete: () => void = () => {}) {
        Animated.timing(this.coordinates.x, {
            toValue: toValue,
            duration: 100,
            easing: Easing.linear,
            useNativeDriver: true
        }).start(() => {
            onMoveComplete();
            this.coordinates.setValue({ x: 0, y: 0 });
            this.canSwipe = true;
        });
    }

    //----------------------------------------------------------------------------
    private onLayoutContainer = (e: LayoutChangeEvent) => {
        this.setState({
            width: e.nativeEvent.layout.width,
            height: e.nativeEvent.layout.height
        });
    };
}
