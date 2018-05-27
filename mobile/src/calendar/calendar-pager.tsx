import React, { Component, Ref, PureComponent } from 'react';
import moment, { Moment } from 'moment';
import { calendarStyles } from './styles';
import { View, TouchableOpacity, Animated, PanResponder, PanResponderInstance, LayoutChangeEvent, Easing, StyleSheet, TransformsStyle, ViewStyle, TranslateXTransform, PanResponderGestureState } from 'react-native';
import { StyledText } from '../override/styled-text';
import { CalendarPage, OnSelectedDayCallback } from './calendar-page';
import { WeekModel, DayModel, CalendarSelection, ReadOnlyIntervalsModel, CalendarPageModel } from '../reducers/calendar/calendar.model';

interface CalendarPagerDefaultProps {
    intervals?: ReadOnlyIntervalsModel;
    selection?: CalendarSelection;
    disableBefore?: DayModel;
}

interface CalendarPagerProps extends CalendarPagerDefaultProps {
    pages: CalendarPageModel[];
    onSelectedDay: OnSelectedDayCallback;
    onNextPage: () => void;
    onPrevPage: () => void;
}

interface CalendarPagerState {
    width: number;
    height: number;
    offset: Animated.ValueXY;
}

export class CalendarPager extends Component<CalendarPagerProps, CalendarPagerState> {
    public static defaultProps: CalendarPagerDefaultProps = {
        intervals: null,
        disableBefore: null
    };

    private panResponder: PanResponderInstance;

    constructor(props: CalendarPagerProps) {
        super(props);
        this.state = {
            width: 0,
            height: 0,
            offset: new Animated.ValueXY({ x: 0, y: 0 })
        };
    }

    public componentWillMount() {
        let canSwipe = true;

        this.panResponder = PanResponder.create({
            onMoveShouldSetPanResponderCapture: (evt, gestureState) => canSwipe,
            onPanResponderMove: Animated.event(
                [
                    null,
                    { dx: this.state.offset.x, dy: this.state.offset.y }
                ]
            ),
            onPanResponderGrant: (e, gesture) => {
                this.state.offset.extractOffset();
            },
            onPanResponderRelease: (e, gesture) => {
                if (this.rightToLeftSwipe(gesture)) {
                    canSwipe = false;
                    this.nextPage();
                    this.state.offset.setValue({ x: this.state.width - Math.abs(gesture.dx), y: 0 });

                    this.moveToNearestPage(() => {
                        canSwipe = true;
                    });
                } else if (this.leftToRightSwipe(gesture)) {
                    canSwipe = false;
                    this.prevPage();
                    this.state.offset.setValue({ x: -(this.state.width - gesture.dx), y: 0 });

                    this.moveToNearestPage(() => {
                        canSwipe = true;
                    });
                }
            }
        });
    }

    public render() {
        const [
            translateXProperty,
            translateYProperty
        ] = this.state.offset.getTranslateTransform() as [{'translateX': Animated.Animated }, {'translateY': Animated.Animated }];

        const swipeableViewStyles = StyleSheet.flatten([
            calendarStyles.swipeableList,
            { transform: [translateXProperty as TranslateXTransform] },
            { left: -this.state.width }
        ]);

        return <View style={calendarStyles.pagerContainer} onLayout={this.onLayoutContainer}>
            <Animated.View
                {...this.panResponder.panHandlers}
                style={swipeableViewStyles}>
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
                        />
                    )
                }
            </Animated.View>
        </View>;
    }

    private rightToLeftSwipe(gesture: PanResponderGestureState): boolean {
        return gesture.dx < 0;
    }

    private leftToRightSwipe(gesture: PanResponderGestureState): boolean {
        return gesture.dx > 0;
    }

    private nextPage() {
        this.props.onNextPage();
    }

    private prevPage() {
        this.props.onPrevPage();
    }

    private moveToNearestPage(completeCallback: () => void) {
        Animated.timing(this.state.offset.x, {
            toValue: 0,
            duration: 200,
            easing: Easing.linear,
            useNativeDriver: true
        }).start(() => {
            completeCallback();
        });
    }

    private onLayoutContainer = (e: LayoutChangeEvent) => {
        this.setState({
            width: e.nativeEvent.layout.width,
            height: e.nativeEvent.layout.height
        });
    }
}