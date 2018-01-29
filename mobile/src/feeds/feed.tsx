import React from 'react';
import { TouchableHighlight, StyleSheet, Platform, Text, View, Image } from 'react-native';

import { Feed } from '../reducers/organization/feed.model';

interface FeedListItemProps {
    message: Feed;
    id: string;
}

const styles = StyleSheet.create({
    layout: {
        flex: 1,
        flexDirection: 'row',
        justifyContent: 'center',
        paddingTop: 5,
        paddingBottom: 5
    },
    img: {
        width: 55,
        height: 55,
        marginTop: 40,
        resizeMode: 'stretch'
    },
    info: {
        flex: 1,
        flexGrow: 1,
        flexDirection: 'column',
        alignSelf: 'center',
        paddingLeft: 13
    },
    title: {
        fontSize: 19,
        textAlign: 'left',
        fontWeight: '500',
        letterSpacing: 2
    },
    text: {
        //marginTop: Platform.OS === 'ios' ? 40 : 20,
        fontSize: 15,
        textAlign: 'left',
        paddingTop: 5,
        paddingBottom: 5
    },
    date: {
        fontSize: 12
    }
});

const months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];

export class FeedListItem extends React.Component<FeedListItemProps> {
    public render() {
        let date = new Date(this.props.message.datePosted);
        let month = months[date.getMonth()];
        let formattedDate = `${month} ${date.getDate()}, ${date.getFullYear()}`;

        return (
            <TouchableHighlight>
                <View style={styles.layout}>
                    <Image style={styles.img} source={{uri: 'https://image.flaticon.com/icons/png/128/126/126486.png'}}/>
                    <View style={styles.info}>
                        <Text style={styles.title}>{this.props.message.title}</Text>
                        <Text style={styles.text}>{this.props.message.text}</Text>
                        <Text style={styles.date}>{formattedDate}</Text>
                    </View>
                </View>
            </TouchableHighlight>
        );
    }
}